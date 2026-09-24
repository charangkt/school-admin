using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolAdmin.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStaffStudentLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_StaffId",
                table: "Users",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StudentId",
                table: "Users",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Staff_StaffId",
                table: "Users",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Students_StudentId",
                table: "Users",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Staff_StaffId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Students_StudentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_StaffId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_StudentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Users");
        }
    }
}
