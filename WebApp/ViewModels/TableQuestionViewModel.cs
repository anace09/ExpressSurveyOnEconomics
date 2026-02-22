namespace WebApp.ViewModels
{
    public class TableQuestionViewModel : QuestionViewModel
    {
        public List<string> Headers { get; set; } = new();
        public List<List<string>> TableData { get; set; } = new();
        public List<int> EditableColumns { get; set; } = new();
    }
}