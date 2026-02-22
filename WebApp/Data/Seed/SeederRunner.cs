using WebApp.Data.SeedData;

namespace WebApp.Data.Seed
{
    public class SeederRunner
    {
        public static void Run(AppDbContext db)
        {
            new TestTasksSeeder().Seed(db);
            new QuestionsSeeder().Seed(db);
        }
    }
}
