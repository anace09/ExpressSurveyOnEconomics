namespace WebApp.ViewModels
{
    public class MatchingQuestionViewModel : QuestionViewModel
    {
        public List<string> LeftItems { get; set; } = new();
        public List<string> RightItems { get; set; } = new();
        public List<int> RightItemIndices { get; set; } = new();
    }
}