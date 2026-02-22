using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Helpers;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class TestsController : Controller
    {

        private readonly ITestEngine _engine;
        private readonly ITestAccess _access;
        private readonly AppDbContext _context;
        private readonly TaskResultViewModelFactory _taskResultFactory;

        public TestsController(ITestEngine engine, ITestAccess access, AppDbContext context, TaskResultViewModelFactory taskResultFactory)
        {
            _context = context;
            _engine = engine;
            _access = access;
            _taskResultFactory = taskResultFactory;
        }

        public IActionResult Index()
        {
            var participantId = HttpContext.Session.GetString("ParticipantId");
            if (string.IsNullOrEmpty(participantId))
                return RedirectToAction("Index", "Start");

            var name = HttpContext.Session.GetString("ParticipantName") ?? "Гость";
            ViewBag.ParticipantName = name;

            var allTasks = _context.TestTasks
                .Include(t => t.Questions)
                .OrderBy(t => t.Number)
                .ToList();

            var completedTaskIds = _context.UserAnswers
                .Where(a => a.ParticipantId == participantId)
                .Select(a => a.TaskId)
                .Distinct()
                .ToHashSet();

            var firstUncompleted = allTasks
                .FirstOrDefault(t => !completedTaskIds.Contains(t.Id));

            var pointsByTask = _context.UserAnswers
                .Where(a => a.ParticipantId == participantId && a.IsCorrect)
                .Select(a => new { a.TaskId, a.SelectedAnswer })
                .ToList()
                .GroupBy(a => a.TaskId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(a =>
                        (a.SelectedAnswer.StartsWith("Оптимальное Q:") || a.SelectedAnswer.StartsWith("Оптимальное P:"))
                            ? 3
                            : 1
                    )
                );


            var tasksViewModel = allTasks.Select(t => new TestTaskViewModel
            {
                Id = t.Id,
                Number = t.Number,
                
                Title = LocalizationHelper.Pick(t.TitleKk, t.TitleRu),
                Description = LocalizationHelper.Pick(t.DescriptionKk, t.DescriptionRu),
                TimeLimit = t.TimeLimitSeconds,
                MaxPoints = t.MaxPoints,
                QuestionCount = t.Questions.Any(q => q is MatchingQuestion)
                    ? t.Questions.OfType<MatchingQuestion>().First().LeftItemsRu?.Count ?? t.Questions.Count
                    : t.Questions.Count,
                IsCompleted = completedTaskIds.Contains(t.Id),
                IsLocked = t.Number > 1 && !allTasks
                   .Where(prev => prev.Number < t.Number)
                   .All(prev => completedTaskIds.Contains(prev.Id)),
                EarnedPoints = pointsByTask.TryGetValue(t.Id, out var p) ? p : 0
            }).ToList();

            ViewBag.FirstUncompletedTaskId = firstUncompleted?.Id;

            return View(tasksViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> TaskPreview(int id)
        {

            var participantId = HttpContext.Session.GetString("ParticipantId");
            if (string.IsNullOrEmpty(participantId))
                return RedirectToAction("Index", "Start");

            if (!_access.CanAccess(id, participantId))
            {
                TempData["ErrorMessage"] = "Это задание пока недоступно. Пройдите предыдущие задания сначала.";
                return RedirectToAction("Index");
            }

            var task = _context.TestTasks
                .Include(t => t.Questions.OrderBy(q => q.Order))
                .FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound();

            var vm = new TaskPreviewViewModel
            {
                Id = task.Id,
                Number = task.Number,
                Title = LocalizationHelper.Pick(task.TitleKk, task.TitleRu),
                Description = LocalizationHelper.Pick(task.DescriptionKk, task.DescriptionRu),
                TimeLimitFormatted = TimeFormatter.Format(task.TimeLimitSeconds),
                CorrectPointsFormatted = PointsFormatter.Format(1, CultureInfo.CurrentUICulture.Name),
                IncorrectPointsFormatted = PointsFormatter.Format(0, CultureInfo.CurrentUICulture.Name),
                MaxPointsFormatted = PointsFormatter.Format(task.MaxPoints, CultureInfo.CurrentUICulture.Name),
                MaxPoints = task.MaxPoints,
                Questions = task.Questions
                    .Shuffle()
                    .Select(QuestionViewModelFactory.Build)
                    .ToList()
            };


            return View("TaskPreview", vm);
        }

        [HttpGet]
        public IActionResult StartTask(int id)
        {
            var participantId = HttpContext.Session.GetString("ParticipantId");
            if (string.IsNullOrEmpty(participantId))
                return RedirectToAction("Index", "Start");

            var task = _context.TestTasks
                .Include(t => t.Questions)
                .FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound();

            // Проверка доступа
            if (!_access.CanAccess(id, participantId))
            {
                TempData["ErrorMessage"] = "Это задание пока недоступно. Пройдите предыдущие задания сначала.";
                return RedirectToAction("Index");
            }

            // Проверка, пройдено ли уже
            var userAnswers = _context.UserAnswers
                .Where(a => a.TaskId == id && a.ParticipantId == participantId)
                .ToList();

            if (userAnswers.Any())
            {
                var earnedPoints = userAnswers.Sum(a => a.IsCorrect ? 1 : 0); 
                var vm = _taskResultFactory.Create(task, userAnswers);
                return View("TaskResult", vm);
            }

            // Подготовка к старту теста
            if (task.Questions == null)
                task.Questions = new List<TaskQuestion>();

            var sessionKey = $"TaskEndTime_{id}_{participantId}";
            if (HttpContext.Session.GetString(sessionKey) == null)
            {
                var endTime = DateTime.UtcNow.AddSeconds(task.TimeLimitSeconds);
                HttpContext.Session.SetString(sessionKey, endTime.ToString("o"));
            }

            var startViewModel = new TaskStartViewModel
            {
                Id = task.Id,
                Title = LocalizationHelper.Pick(task.TitleKk, task.TitleRu),
                Description = LocalizationHelper.Pick(task.DescriptionKk, task.DescriptionRu),
                TimeLimitSeconds = task.TimeLimitSeconds,
                EndTimeUtc = HttpContext.Session.GetString(sessionKey),
                Questions = task.Questions
                    .Shuffle()
                    .Select(QuestionViewModelFactory.Build)
                    .ToList()
            };

            return View("TaskStart", startViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTaskAnswers(int taskId, IFormCollection form)
        {
            var participantId = HttpContext.Session.GetString("ParticipantId");
            if (string.IsNullOrEmpty(participantId))
                return RedirectToAction("Index", "Start");

            var result = await _engine.CheckAsync(taskId, participantId, form);

            return View("TaskResult", result);
        }

        [HttpGet]
        public IActionResult TaskResult(int id)
        {
            var participantId = HttpContext.Session.GetString("ParticipantId");
            if (string.IsNullOrEmpty(participantId))
                return RedirectToAction("Index", "Start");

            var task = _context.TestTasks.Find(id);
            if (task == null)
                return NotFound();

            var userAnswers = _context.UserAnswers
                .Where(a => a.TaskId == id && a.ParticipantId == participantId)
                .ToList();

            if (!userAnswers.Any())
            {
                TempData["ErrorMessage"] = "Вы ещё не проходили это задание.";
                return RedirectToAction("Index");
            }

            var vm = _taskResultFactory.Create(task, userAnswers);

            return View(vm);
        }

    }

}
