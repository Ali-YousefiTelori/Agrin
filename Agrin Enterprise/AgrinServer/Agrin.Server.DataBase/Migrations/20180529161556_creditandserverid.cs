using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agrin.Server.DataBase.Migrations
{
    public partial class creditandserverid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServerName",
                table: "DirectFileInfoes");

            migrationBuilder.AddColumn<Guid>(
                name: "Key",
                table: "UserCreditInfoes",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "DirectFileInfoes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServerInfoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Domain = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerInfoes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCreditInfoes_Key",
                table: "UserCreditInfoes",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectFileInfoes_ServerId",
                table: "DirectFileInfoes",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFileInfoes_ServerInfoes_ServerId",
                table: "DirectFileInfoes",
                column: "ServerId",
                principalTable: "ServerInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectFileInfoes_ServerInfoes_ServerId",
                table: "DirectFileInfoes");

            migrationBuilder.DropTable(
                name: "ServerInfoes");

            migrationBuilder.DropIndex(
                name: "IX_UserCreditInfoes_Key",
                table: "UserCreditInfoes");

            migrationBuilder.DropIndex(
                name: "IX_DirectFileInfoes_ServerId",
                table: "DirectFileInfoes");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "UserCreditInfoes");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "DirectFileInfoes");

            migrationBuilder.AddColumn<string>(
                name: "ServerName",
                table: "DirectFileInfoes",
                nullable: true);
        }
    }
}
