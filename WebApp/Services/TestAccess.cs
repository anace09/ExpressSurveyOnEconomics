using WebApp.Data;

public class TestAccess : ITestAccess
{
    private readonly AppDbContext _context;

    public TestAccess(AppDbContext context)
    {
        _context = context;
    }

    public bool CanAccess(int taskId, string participantId)
    {
        var task = _context.TestTasks.Find(taskId);
        if (task == null) return false;

        if (task.Number <= 1) return true;

        var previousTasks = _context.TestTasks
            .Where(t => t.Number < task.Number)
            .Select(t => t.Id)
            .ToList();

        if (!previousTasks.Any()) return true;

        var completedPrevious = _context.UserAnswers
            .Where(a => a.ParticipantId == participantId && previousTasks.Contains(a.TaskId))
            .Select(a => a.TaskId)
            .Distinct()
            .Count();

        return completedPrevious == previousTasks.Count;
    }
}
