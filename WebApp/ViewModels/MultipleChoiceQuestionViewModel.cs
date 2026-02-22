namespace WebApp.ViewModels
{
    public class MultipleChoiceQuestionViewModel : QuestionViewModel
    {
        public List<string> Options { get; set; } = new();
        public List<int> OptionIndices { get; set; } = new();
    }
}