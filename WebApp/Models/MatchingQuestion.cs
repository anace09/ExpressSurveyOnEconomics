namespace WebApp.Models
{
    public class MatchingQuestion : TaskQuestion
    {

        public List<string> LeftItemsRu { get; set; } = new();
        public List<string> LeftItemsKk { get; set; } = new();

        public List<string> RightItemsRu { get; set; } = new();
        public List<string> RightItemsKk { get; set; } = new();

        public Dictionary<int, int> CorrectMatches { get; set; } = new();

    }
}
