using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillProof.Data.Migrations
{
    /// <inheritdoc />
    public partial class AbilityToSaveJobsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
