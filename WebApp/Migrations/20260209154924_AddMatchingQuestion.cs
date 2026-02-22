using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchingQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectMatches",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeftItemsKk",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeftItemsRu",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RightItemsKk",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RightItemsRu",
                table: "TaskQuestions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectMatches",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "LeftItemsKk",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "LeftItemsRu",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "RightItemsKk",
                table: "TaskQuestions");

            migrationBuilder.DropColumn(
                name: "RightItemsRu",
                table: "TaskQuestions");
        }
    }
}
