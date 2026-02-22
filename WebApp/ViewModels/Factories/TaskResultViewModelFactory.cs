using WebApp.Helpers;
using WebApp.Models;
using WebApp.ViewModels;

public class TaskResultViewModelFactory
{
    public TaskResultViewModel Create(TestTask task, IEnumerable<UserAnswer> answers)
    {
        var earned = answers.Sum(a => a.IsCorrect ? 1 : 0);

        return new TaskResultViewModel
        {
            TaskId = task.Id,
            Title = LocalizationHelper.Pick(task.TitleKk, task.TitleRu),
            EarnedPoints = earned,
            MaxPoints = task.MaxPoints,
            Message = BuildMessage(earned, task.MaxPoints)
        };
    }

    private string BuildMessage(int earned, int max)
    {
        if (earned >= max) return "Отлично! Полный балл!";
        return $"Вы набрали {earned} из {max}";
    }
}
