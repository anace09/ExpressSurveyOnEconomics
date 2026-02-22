using WebApp.Models;

namespace WebApp.ViewModels
{
    public class TaskResultViewModel
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public int EarnedPoints { get; set; }
        public int MaxPoints { get; set; }
        public string Message { get; set; }
        public List<UserAnswer> Answers { get; set; } = new List<UserAnswer>();
    }
}
