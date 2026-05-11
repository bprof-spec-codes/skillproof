using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillProof.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureSavedJobsManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_AspNetUsers_UsersId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_UsersId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Jobs");

            migrationBuilder.CreateTable(
                name: "UserSavedJobs",
                columns: table => new
                {
                    SavedJobsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSavedJobs", x => new { x.SavedJobsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_UserSavedJobs_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSavedJobs_Jobs_SavedJobsId",
                        column: x => x.SavedJobsId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSavedJobs_UsersId",
                table: "UserSavedJobs",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSavedJobs");

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "Jobs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_UsersId",
                table: "Jobs",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_AspNetUsers_UsersId",
                table: "Jobs",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
