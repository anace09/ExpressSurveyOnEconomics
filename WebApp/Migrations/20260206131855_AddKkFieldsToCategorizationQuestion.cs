using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddKkFieldsToCategorizationQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskQuestions_TestTasks_TestTaskId",
                table: "TaskQuestions");

            migrationBuilder.DropIndex(
                name: "IX_TaskQuestions_TestTaskId",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "TestTaskId",
                table: "TaskQuestions");

            migrationBuilder.AddColumn<string>(
                name: "Category1Kk",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category1Ru",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category2Kk",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category2Ru",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectMapping",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermsKk",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermsRu",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_TaskId",
                table: "UserAnswers",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_TestTasks_TaskId",
                table: "UserAnswers",
                column: "TaskId",
                principalTable: "TestTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_TestTasks_TaskId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_TaskId",
                table: "UserAnswers");

            migrationBuilder.DropColumn(
                name: "Category1Kk",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "Category1Ru",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "Category2Kk",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "Category2Ru",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "CorrectMapping",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "TermsKk",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "TermsRu",
                table: "TaskQuestions");

            migrationBuilder.AddColumn<int>(
                name: "TestTaskId",
                table: "TaskQuestions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskQuestions_TestTaskId",
                table: "TaskQuestions",
                column: "TestTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskQuestions_TestTasks_TestTaskId",
                table: "TaskQuestions",
                column: "TestTaskId",
                principalTable: "TestTasks",
                principalColumn: "Id");
        }
    }
}
