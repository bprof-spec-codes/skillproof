using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillProof.Data.Migrations
{
    /// <inheritdoc />
    public partial class AssesmentPropertyAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SkillAssessments_Skills_SkillId",
                table: "SkillAssessments");

            migrationBuilder.RenameColumn(
                name: "SkillId",
                table: "SkillAssessments",
                newName: "SkillsId");

            migrationBuilder.RenameIndex(
                name: "IX_SkillAssessments_SkillId",
                table: "SkillAssessments",
                newName: "IX_SkillAssessments_SkillsId");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillAssessments_Skills_SkillsId",
                table: "SkillAssessments",
                column: "SkillsId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SkillAssessments_Skills_SkillsId",
                table: "SkillAssessments");

            migrationBuilder.RenameColumn(
                name: "SkillsId",
                table: "SkillAssessments",
                newName: "SkillId");

            migrationBuilder.RenameIndex(
                name: "IX_SkillAssessments_SkillsId",
                table: "SkillAssessments",
                newName: "IX_SkillAssessments_SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillAssessments_Skills_SkillId",
                table: "SkillAssessments",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
