using WebApp.Models;

namespace WebApp.ViewModels
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public int Points { get; set; }
    }
}