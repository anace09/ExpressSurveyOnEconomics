namespace WebApp.ViewModels.Factories
{
    public class AdminResultViewModel
    {
        public string ParticipantId { get; set; } = "";
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = "";
        public int EarnedPoints { get; set; }
        public int TotalAnswers { get; set; }
        public DateTime AnsweredAt { get; set; }
        public string FullName { get; set; } = "";
        public string Organization { get; set; } = "";
    }
}
