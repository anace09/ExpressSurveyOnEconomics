using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTestTasksAndQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    TitleRu = table.Column<string>(type: "TEXT", nullable: false),
                    TitleKk = table.Column<string>(type: "TEXT", nullable: false),
                    DescriptionRu = table.Column<string>(type: "TEXT", nullable: false),
                    DescriptionKk = table.Column<string>(type: "TEXT", nullable: false),
                    TimeLimitSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxPoints = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TaskId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParticipantId = table.Column<string>(type: "TEXT", nullable: false),
                    SelectedAnswer = table.Column<string>(type: "TEXT", nullable: false),
                    IsCorrect = table.Column<bool>(type: "INTEGER", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TaskId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    TextRu = table.Column<string>(type: "TEXT", nullable: false),
                    TextKk = table.Column<string>(type: "TEXT", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionType = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false),
                    TestTaskId = table.Column<int>(type: "INTEGER", nullable: true),
                    OptionsRu = table.Column<string>(type: "TEXT", nullable: true),
                    OptionsKk = table.Column<string>(type: "TEXT", nullable: true),
                    CorrectOptionIndex = table.Column<int>(type: "INTEGER", nullable: true),
                    CorrectAnswer = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskQuestions_TestTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "TestTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskQuestions_TestTasks_TestTaskId",
                        column: x => x.TestTaskId,
                        principalTable: "TestTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_FirstName_LastName_MiddleName_Organization",
                table: "Persons",
                columns: new[] { "FirstName", "LastName", "MiddleName", "Organization" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskQuestions_TaskId",
                table: "TaskQuestions",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskQuestions_TestTaskId",
                table: "TaskQuestions",
                column: "TestTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskQuestions");

            migrationBuilder.DropTable(
                name: "UserAnswers");

            migrationBuilder.DropTable(
                name: "TestTasks");

            migrationBuilder.DropIndex(
                name: "IX_Persons_FirstName_LastName_MiddleName_Organization",
                table: "Persons");
        }
    }
}
