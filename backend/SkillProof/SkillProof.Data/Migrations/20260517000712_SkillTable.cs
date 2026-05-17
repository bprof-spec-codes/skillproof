using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillProof.Data.Migrations
{
    /// <inheritdoc />
    public partial class SkillTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Skill_SkillId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillUsers_AspNetUsers_UsersId",
                table: "SkillUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillUsers_Skill_SkillsId",
                table: "SkillUsers");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_SkillId",
                table: "Assessments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillUsers",
                table: "SkillUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skill",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "SkillId",
                table: "Assessments");

            migrationBuilder.RenameTable(
                name: "SkillUsers",
                newName: "UserSkills");

            migrationBuilder.RenameTable(
                name: "Skill",
                newName: "Skills");

            migrationBuilder.RenameIndex(
                name: "IX_SkillUsers_UsersId",
                table: "UserSkills",
                newName: "IX_UserSkills_UsersId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSkills",
                table: "UserSkills",
                columns: new[] { "SkillsId", "UsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skills",
                table: "Skills",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SkillAssessments",
                columns: table => new
                {
                    AssessmentsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SkillId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillAssessments", x => new { x.AssessmentsId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_SkillAssessments_Assessments_AssessmentsId",
                        column: x => x.AssessmentsId,
                        principalTable: "Assessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillAssessments_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkillAssessments_SkillId",
                table: "SkillAssessments",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkills_AspNetUsers_UsersId",
                table: "UserSkills",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkills_Skills_SkillsId",
                table: "UserSkills",
                column: "SkillsId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSkills_AspNetUsers_UsersId",
                table: "UserSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSkills_Skills_SkillsId",
                table: "UserSkills");

            migrationBuilder.DropTable(
                name: "SkillAssessments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSkills",
                table: "UserSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skills",
                table: "Skills");

            migrationBuilder.RenameTable(
                name: "UserSkills",
                newName: "SkillUsers");

            migrationBuilder.RenameTable(
                name: "Skills",
                newName: "Skill");

            migrationBuilder.RenameIndex(
                name: "IX_UserSkills_UsersId",
                table: "SkillUsers",
                newName: "IX_SkillUsers_UsersId");

            migrationBuilder.AddColumn<string>(
                name: "SkillId",
                table: "Assessments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skill",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillUsers",
                table: "SkillUsers",
                columns: new[] { "SkillsId", "UsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skill",
                table: "Skill",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_SkillId",
                table: "Assessments",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Skill_SkillId",
                table: "Assessments",
                column: "SkillId",
                principalTable: "Skill",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillUsers_AspNetUsers_UsersId",
                table: "SkillUsers",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SkillUsers_Skill_SkillsId",
                table: "SkillUsers",
                column: "SkillsId",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
