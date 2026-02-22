using WebApp.Helpers;
using WebApp.Models;
using WebApp.ViewModels;

public class TestTaskViewModelFactory
{
    public TestTaskViewModel Create(
        TestTask task,
        HashSet<int> completed,
        Dictionary<int, int> points,
        List<TestTask> allTasks)
    {
        return new TestTaskViewModel
        {
            Id = task.Id,
            Number = task.Number,
            Title = LocalizationHelper.Pick(task.TitleKk, task.TitleRu),
            Description = LocalizationHelper.Pick(task.DescriptionKk, task.DescriptionRu),
            TimeLimit = task.TimeLimitSeconds,
            MaxPoints = task.MaxPoints,
            QuestionCount = task.Questions.Count,
            IsCompleted = completed.Contains(task.Id),
            IsLocked = task.Number > 1 &&
                allTasks.Where(p => p.Number < task.Number)
                        .Any(p => !completed.Contains(p.Id)),
            EarnedPoints = points.TryGetValue(task.Id, out var p) ? p : 0
        };
    }
}
