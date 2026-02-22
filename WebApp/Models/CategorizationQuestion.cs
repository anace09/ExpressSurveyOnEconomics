namespace WebApp.Models
{
    public class CategorizationQuestion : TaskQuestion
    {
        public string Category1Ru { get; set; }
        public string Category2Ru { get; set; }

        public string Category1Kk { get; set; }
        public string Category2Kk { get; set; }

        public List<string> TermsRu { get; set; } = new();
        public List<string> TermsKk { get; set; } = new();

        public Dictionary<int, int> CorrectMapping { get; set; } = new();
    }
}