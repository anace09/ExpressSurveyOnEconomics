namespace WebApp.ViewModels
{
    public class TaskPreviewViewModel
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string TimeLimitFormatted { get; set; }
        public required string CorrectPointsFormatted { get; set; }
        public required string IncorrectPointsFormatted { get; set; }
        public required string MaxPointsFormatted { get; set; }
        public int MaxPoints { get; set; }

        public List<QuestionViewModel> Questions { get; set; } = new();
    }

}
