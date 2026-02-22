using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Models;
using WebApp.ViewModels;
using WebApp.Helpers;

public class TestEngineService : ITestEngine
{
    private readonly AppDbContext _context;

    public TestEngineService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskResultViewModel> CheckAsync(int taskId, string participantId, IFormCollection form)
    {
        var task = await _context.TestTasks
            .Include(t => t.Questions)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
            throw new Exception("Task not found");

        int earnedPoints = 0;

        foreach (var q in task.Questions)
        {
            if (q is TrueFalseQuestion tf)
            {
                string val = form[$"answers[{q.Id}]"].ToString();

                bool? userAnswer = null;
                if (!string.IsNullOrEmpty(val))
                {
                    bool.TryParse(val, out bool parsed);
                    userAnswer = parsed;
                }

                bool isCorrect = userAnswer.HasValue && userAnswer.Value == tf.CorrectAnswer;

                // Если ничего не выбрано — 0 баллов
                if (!userAnswer.HasValue)
                {
                    _context.UserAnswers.Add(new UserAnswer
                    {
                        TaskId = taskId,
                        ParticipantId = participantId,
                        SelectedAnswer = "не выбрано",
                        IsCorrect = false,
                        AnsweredAt = DateTime.UtcNow
                    });
                }
                else
                {
                    if (isCorrect) earnedPoints += q.Points;

                    _context.UserAnswers.Add(new UserAnswer
                    {
                        TaskId = taskId,
                        ParticipantId = participantId,
                        SelectedAnswer = val,
                        IsCorrect = isCorrect,
                        AnsweredAt = DateTime.UtcNow
                    });
                }
            }
            else if (q is MultipleChoiceQuestion mc)
            {
                string val = form[$"answers[{q.Id}]"];
                bool isCorrect = false;
                if (int.TryParse(val, out int index))
                {
                    isCorrect = index == mc.CorrectOptionIndex;
                    if (isCorrect) earnedPoints += q.Points;
                }

                _context.UserAnswers.Add(new UserAnswer
                {
                    TaskId = taskId,
                    ParticipantId = participantId,
                    SelectedAnswer = val ?? "не выбрано",
                    IsCorrect = isCorrect,
                    AnsweredAt = DateTime.UtcNow
                });
            }
            else if (q is CategorizationQuestion cat)
            {
                int correctCount = 0;

                string json = form["categorizationAnswers"];

                if (string.IsNullOrEmpty(json) || json == "{}")
                {
                    _context.UserAnswers.Add(new UserAnswer
                    {
                        TaskId = taskId,
                        ParticipantId = participantId,
                        SelectedAnswer = "не отвечено",
                        IsCorrect = false,
                        AnsweredAt = DateTime.UtcNow
                    });
                }
                else
                {
                    var answersRaw = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    foreach (var kv in answersRaw)
                    {
                        if (int.TryParse(kv.Key, out int termIndex) && int.TryParse(kv.Value, out int selectedCategory))
                        {
                            bool termCorrect = cat.CorrectMapping.TryGetValue(termIndex, out int expected) && selectedCategory == expected;

                            if (termCorrect)
                                correctCount++;

                            // Сохраняем отдельную запись на каждый термин
                            _context.UserAnswers.Add(new UserAnswer
                            {
                                TaskId = taskId,
                                ParticipantId = participantId,
                                SelectedAnswer = kv.Value, // выбранная категория
                                IsCorrect = termCorrect,
                                AnsweredAt = DateTime.UtcNow
                            });
                        }
                    }
                }

                earnedPoints += correctCount; // по 1 баллу за каждый правильный термин
            }
            else if (q is MatchingQuestion match)
            {
                string jsonInput = form["matchingAnswers[" + q.Id + "]"];

                if (!string.IsNullOrEmpty(jsonInput))
                {
                    try
                    {
                        // View шлёт массив: [2, 0, 1, 3]
                        // Индекс в массиве = позиция левого элемента (0..N-1)
                        // Значение = индекс правого элемента который пользователь поставил напротив
                        var userOrder = System.Text.Json.JsonSerializer.Deserialize<int[]>(jsonInput);
                        int correctCount = 0;

                        for (int leftIndex = 0; leftIndex < userOrder.Length; leftIndex++)
                        {
                            int userRightIndex = userOrder[leftIndex];
                            bool isCorrect = match.CorrectMatches.TryGetValue(leftIndex, out int expectedRight)
                                             && userRightIndex == expectedRight;

                            if (isCorrect) correctCount++;

                            _context.UserAnswers.Add(new UserAnswer
                            {
                                TaskId = taskId,
                                ParticipantId = participantId,
                                SelectedAnswer = $"Left {leftIndex} → Right {userRightIndex}",
                                IsCorrect = isCorrect,
                                AnsweredAt = DateTime.UtcNow
                            });
                        }

                        earnedPoints += correctCount;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка парсинга matching: " + ex.Message);
                    }
                }
            }
            else if (q is TableQuestion table)
            {
                int correctCells = 0;

                int lastEditableCol = table.EditableColumns?.Any() == true
                    ? table.EditableColumns.Max()
                    : -1;

                for (int row = 1; row < table.TableDataRu.Count; row++)
                {
                    int correctRowIndex = row - 1;

                    for (int i = 0; i < table.EditableColumns.Count; i++)
                    {
                        int col = table.EditableColumns[i];
                        string key = $"tableAnswers[{q.Id}][{row}][{col}]";
                        string userValue = form[key].FirstOrDefault()?.Trim() ?? "";
                        string expected = table.CorrectAnswers[correctRowIndex][i];

                        bool isCorrect;

                        if (col == lastEditableCol)
                        {
                            // Последняя колонка — MC>MR / MC<MR / MC=MR
                            // Сравниваем без ToLowerInvariant, строго как есть
                            isCorrect = userValue == expected;
                        }
                        else
                        {
                            // Числовые ячейки — без учёта регистра и пробелов
                            isCorrect = userValue.ToLowerInvariant() == expected.ToLowerInvariant();
                        }

                        if (isCorrect) correctCells++;

                        _context.UserAnswers.Add(new UserAnswer
                        {
                            TaskId = taskId,
                            ParticipantId = participantId,
                            SelectedAnswer = $"Строка {row}, столбец {col + 1}: '{userValue}' (ожидалось: '{expected}')",
                            IsCorrect = isCorrect,
                            AnsweredAt = DateTime.UtcNow
                        });
                    }
                }

                // Проверка финальных Q и P
                string userQ = form[$"optimalQ[{q.Id}]"].FirstOrDefault()?.Trim() ?? "";
                string userP = form[$"optimalP[{q.Id}]"].FirstOrDefault()?.Trim() ?? "";

                bool qCorrect = userQ.ToLowerInvariant() == table.CorrectOptimalQ.ToLowerInvariant();
                bool pCorrect = userP.ToLowerInvariant() == table.CorrectOptimalP.ToLowerInvariant();

                _context.UserAnswers.Add(new UserAnswer
                {
                    TaskId = taskId,
                    ParticipantId = participantId,
                    SelectedAnswer = $"Оптимальное Q: '{userQ}' (ожидалось: '{table.CorrectOptimalQ}')",
                    IsCorrect = qCorrect,
                    Points = 3,
                    AnsweredAt = DateTime.UtcNow
                });

                _context.UserAnswers.Add(new UserAnswer
                {
                    TaskId = taskId,
                    ParticipantId = participantId,
                    SelectedAnswer = $"Оптимальное P: '{userP}' (ожидалось: '{table.CorrectOptimalP}')",
                    IsCorrect = pCorrect,
                    Points = 3,
                    AnsweredAt = DateTime.UtcNow
                });

                int finalPoints = (qCorrect ? 3 : 0) + (pCorrect ? 3 : 0);
                earnedPoints += correctCells + finalPoints;
            }
        }

        await _context.SaveChangesAsync();

        return new TaskResultViewModel
        {
            TaskId = task.Id,
            Title = LocalizationHelper.Pick(task.TitleKk, task.TitleRu),
            EarnedPoints = earnedPoints,
            MaxPoints = task.MaxPoints,
            Message = earnedPoints >= task.MaxPoints
                ? "Отлично! Полный балл!"
                : $"Вы набрали {earnedPoints} из {task.MaxPoints}"
        };
    }
}
