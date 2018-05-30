using Microsoft.EntityFrameworkCore.Migrations;

namespace Agrin.Server.DataBase.Migrations
{
    public partial class serverid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectFileInfoes_ServerInfoes_ServerId",
                table: "DirectFileInfoes");

            migrationBuilder.AlterColumn<int>(
                name: "ServerId",
                table: "DirectFileInfoes",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFileInfoes_ServerInfoes_ServerId",
                table: "DirectFileInfoes",
                column: "ServerId",
                principalTable: "ServerInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectFileInfoes_ServerInfoes_ServerId",
                table: "DirectFileInfoes");

            migrationBuilder.AlterColumn<int>(
                name: "ServerId",
                table: "DirectFileInfoes",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFileInfoes_ServerInfoes_ServerId",
                table: "DirectFileInfoes",
                column: "ServerId",
                principalTable: "ServerInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
