namespace WebApp.Models
{
    public class MultipleChoiceQuestion : TaskQuestion
    {
        public List<string> OptionsRu { get; set; } = new();
        public List<string> OptionsKk { get; set; } = new();
        public int CorrectOptionIndex { get; set; }
    }
}