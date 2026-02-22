namespace WebApp.Models
{
    public abstract class TaskQuestion
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int Order { get; set; }
        public string TextRu { get; set; }
        public string TextKk { get; set; }
        public int Points { get; set; } = 1;

        public TestTask Task { get; set; } = null!;

    }
}