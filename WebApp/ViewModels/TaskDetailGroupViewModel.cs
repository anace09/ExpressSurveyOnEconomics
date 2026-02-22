namespace WebApp.ViewModels
{
    public class TaskDetailGroupViewModel
    {
        public WebApp.Models.TestTask Task { get; set; } = null!;
        public List<WebApp.Models.UserAnswer> Answers { get; set; } = new();
        public int EarnedPoints { get; set; }
    }
}
