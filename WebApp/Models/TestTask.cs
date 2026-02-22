using System.Threading.Tasks;

namespace WebApp.Models
{
    public class TestTask
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string TitleRu { get; set; } = "";
        public string TitleKk { get; set; } = "";
        public string DescriptionRu { get; set; } = "";
        public string DescriptionKk { get; set; } = "";
        public int TimeLimitSeconds { get; set; } = 120;
        public int MaxPoints { get; set; } = 3;


        public ICollection<TaskQuestion> Questions { get; set; } = new List<TaskQuestion>();

    }
}