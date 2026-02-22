using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectAnswers",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditableColumns",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadersKk",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadersRu",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TableDataKk",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TableDataRu",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswers",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "EditableColumns",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "HeadersKk",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "HeadersRu",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "TableDataKk",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "TableDataRu",
                table: "TaskQuestions");
        }
    }
}
