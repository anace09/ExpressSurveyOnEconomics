namespace WebApp.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string ParticipantId { get; set; }  
        public string SelectedAnswer { get; set; } 
        public bool IsCorrect { get; set; }
        public int Points { get; set; } = 1;
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

        public TestTask? Task { get; set; }
    }

}
