namespace WebApp.ViewModels
{
    public class CategorizationQuestionViewModel : QuestionViewModel
    {
        public string Category1 { get; set; } = "";
        public string Category2 { get; set; } = "";
        public List<string> Terms { get; set; } = new();
        public List<int> TermIndices { get; set; } = new();
    }
}