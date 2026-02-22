namespace WebApp.Models
{
    public class TableQuestion : TaskQuestion
    {

        public List<string> HeadersRu { get; set; } = new();
        public List<string> HeadersKk { get; set; } = new();

        public List<List<string>> TableDataRu { get; set; } = new();
        public List<List<string>> TableDataKk { get; set; } = new();

        public List<int> EditableColumns { get; set; } = new(); 
        public List<List<string>> CorrectAnswers { get; set; } = new();

        public string CorrectOptimalQ { get; set; } = "5";
        public string CorrectOptimalP { get; set; } = "105";

    }
}
