using Microsoft.EntityFrameworkCore.Migrations;

namespace Agrin.Server.DataBase.Migrations
{
    public partial class fixtelegramidbug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserInfoes_TelegramUserId",
                table: "UserInfoes");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfoes_TelegramUserId",
                table: "UserInfoes",
                column: "TelegramUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserInfoes_TelegramUserId",
                table: "UserInfoes");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfoes_TelegramUserId",
                table: "UserInfoes",
                column: "TelegramUserId",
                unique: true);
        }
    }
}
