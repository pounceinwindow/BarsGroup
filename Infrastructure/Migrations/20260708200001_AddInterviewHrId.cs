using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInterviewHrId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HrId",
                table: "Interviews",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_HrId",
                table: "Interviews",
                column: "HrId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_Users_HrId",
                table: "Interviews",
                column: "HrId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Users_HrId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_HrId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "HrId",
                table: "Interviews");
        }
    }
}
