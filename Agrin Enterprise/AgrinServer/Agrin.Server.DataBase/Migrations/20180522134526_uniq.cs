using Microsoft.EntityFrameworkCore.Migrations;

namespace Agrin.Server.DataBase.Migrations
{
    public partial class uniq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserInfoes_UserName",
                table: "UserInfoes");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfoes_UserName",
                table: "UserInfoes",
                column: "UserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserInfoes_UserName",
                table: "UserInfoes");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfoes_UserName",
                table: "UserInfoes",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");
        }
    }
}
