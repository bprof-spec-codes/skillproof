using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillProof.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssessmentStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assessments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentsJob",
                columns: table => new
                {
                    AssessmentsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JobsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentsJob", x => new { x.AssessmentsId, x.JobsId });
                    table.ForeignKey(
                        name: "FK_AssessmentsJob_Assessments_AssessmentsId",
                        column: x => x.AssessmentsId,
                        principalTable: "Assessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssessmentsJob_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentsQuestions",
                columns: table => new
                {
                    AssessmentsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestionsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentsQuestions", x => new { x.AssessmentsId, x.QuestionsId });
                    table.ForeignKey(
                        name: "FK_AssessmentsQuestions_Assessments_AssessmentsId",
                        column: x => x.AssessmentsId,
                        principalTable: "Assessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssessmentsQuestions_Questions_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentsTests",
                columns: table => new
                {
                    AssessmentsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TestAttemptsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentsTests", x => new { x.AssessmentsId, x.TestAttemptsId });
                    table.ForeignKey(
                        name: "FK_AssessmentsTests_Assessments_AssessmentsId",
                        column: x => x.AssessmentsId,
                        principalTable: "Assessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssessmentsTests_Tests_TestAttemptsId",
                        column: x => x.TestAttemptsId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentsJob_JobsId",
                table: "AssessmentsJob",
                column: "JobsId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentsQuestions_QuestionsId",
                table: "AssessmentsQuestions",
                column: "QuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentsTests_TestAttemptsId",
                table: "AssessmentsTests",
                column: "TestAttemptsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentsJob");

            migrationBuilder.DropTable(
                name: "AssessmentsQuestions");

            migrationBuilder.DropTable(
                name: "AssessmentsTests");

            migrationBuilder.DropTable(
                name: "Assessments");
        }
    }
}
