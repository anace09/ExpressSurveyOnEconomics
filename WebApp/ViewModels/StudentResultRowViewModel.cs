namespace WebApp.ViewModels
{
    public class StudentResultRowViewModel
    {
        public string ParticipantId { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Organization { get; set; } = "";
        public Dictionary<int, int> ScoreByTask { get; set; } = new();
        public int TotalPoints { get; set; }
        public DateTime LastAnsweredAt { get; set; }
    }
}