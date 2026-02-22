using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebApp.Models;

namespace WebApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<TestTask> TestTasks { get; set; }
        public DbSet<TaskQuestion> TaskQuestions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>()
                .HasIndex(p => new { p.FirstName, p.LastName, p.MiddleName, p.Organization })
                .IsUnique();

            modelBuilder.Entity<CategorizationQuestion>()
                .Property(q => q.TermsRu)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)!
                );

            modelBuilder.Entity<CategorizationQuestion>()
                .Property(q => q.CorrectMapping)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<int, int>>(v, (JsonSerializerOptions)null)!
                );

            modelBuilder.Entity<TaskQuestion>()
                .HasDiscriminator<string>("QuestionType")
                .HasValue<TrueFalseQuestion>("TrueFalse")
                .HasValue<MultipleChoiceQuestion>("MultipleChoice")
                .HasValue<CategorizationQuestion>("Categorization")
                .HasValue<MatchingQuestion>("Matching");

            modelBuilder.Entity<TaskQuestion>()
                .HasOne(q => q.Task)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MatchingQuestion>()
                .Property(q => q.LeftItemsRu)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)!
                );

            modelBuilder.Entity<MatchingQuestion>()
                .Property(q => q.RightItemsRu)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)!
                );

            modelBuilder.Entity<MatchingQuestion>()
                .Property(q => q.CorrectMatches)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<int, int>>(v, (JsonSerializerOptions)null)!
                );

            modelBuilder.Entity<TableQuestion>()
                .Property(q => q.TableDataRu)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<List<string>>>(v, (JsonSerializerOptions)null)!
            );

            modelBuilder.Entity<TableQuestion>()
                .Property(q => q.TableDataKk)
                .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                   v => JsonSerializer.Deserialize<List<List<string>>>(v, (JsonSerializerOptions)null)!
                );

            modelBuilder.Entity<TableQuestion>()
                .Property(q => q.CorrectAnswers)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<List<string>>>(v, (JsonSerializerOptions)null)!
                );

            modelBuilder.Entity<TaskQuestion>()
                .HasDiscriminator<string>("QuestionType")
                .HasValue<TableQuestion>("Table");

        }

    }
}
