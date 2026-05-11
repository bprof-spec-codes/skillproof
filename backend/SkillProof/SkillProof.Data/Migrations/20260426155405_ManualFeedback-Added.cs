using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillProof.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManualFeedbackAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManualFeedback",
                table: "TestAnswers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManualFeedback",
                table: "TestAnswers");
        }
    }
}
