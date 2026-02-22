namespace WebApp.ViewModels
{
    public class TestTaskViewModel
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int TimeLimit { get; set; }
        public int MaxPoints { get; set; }
        public int QuestionCount { get; set; }
        public bool IsCompleted { get; set; }
        public int EarnedPoints { get; set; }
        public bool IsLocked { get; set; }
    }
}