namespace WebApp.ViewModels
{
    public class TaskStartViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TimeLimitSeconds { get; set; }
        public List<QuestionViewModel> Questions { get; set; } = new();
 
        public bool AlreadyCompleted { get; set; }
        public int EarnedPoints { get; set; }
        public int MaxPoints { get; set; }
        public string EndTimeUtc { get; set; }
    }
}