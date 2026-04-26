using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillProof.Data.Migrations
{
    /// <inheritdoc />
    public partial class Test_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Score",
                table: "Tests",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "Inspected",
                table: "TestAnswers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "TestAnswers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inspected",
                table: "TestAnswers");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "TestAnswers");

            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "Tests",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
