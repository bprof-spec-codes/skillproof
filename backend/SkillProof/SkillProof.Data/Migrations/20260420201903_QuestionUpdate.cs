using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillProof.Data.Migrations
{
    /// <inheritdoc />
    public partial class QuestionUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Questions");
        }
    }
}
