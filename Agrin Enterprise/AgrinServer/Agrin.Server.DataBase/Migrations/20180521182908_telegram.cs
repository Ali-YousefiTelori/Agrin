using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agrin.Server.DataBase.Migrations
{
    public partial class telegram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCreditInfoes_UserInfoes_UserId",
                table: "UserCreditInfoes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInfoes_UserCreditInfoes_UserCreditId",
                table: "UserInfoes");

            migrationBuilder.DropIndex(
                name: "IX_UserInfoes_UserCreditId",
                table: "UserInfoes");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "UserInfoes");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "UserCreditInfoes");

            migrationBuilder.DropColumn(
                name: "RoamUploadSize",
                table: "UserCreditInfoes");

            migrationBuilder.DropColumn(
                name: "StaticUploadSize",
                table: "UserCreditInfoes");

            migrationBuilder.RenameColumn(
                name: "UserCreditId",
                table: "UserInfoes",
                newName: "TelegramUserId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserCreditInfoes",
                newName: "ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCreditInfoes_UserId",
                table: "UserCreditInfoes",
                newName: "IX_UserCreditInfoes_ToUserId");

            migrationBuilder.AddColumn<long>(
                name: "Credit",
                table: "UserInfoes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "RoamUploadSize",
                table: "UserInfoes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StaticUploadSize",
                table: "UserInfoes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "UserCreditInfoes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FromUserId",
                table: "UserCreditInfoes",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "UserCreditInfoes",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfoes_TelegramUserId",
                table: "UserInfoes",
                column: "TelegramUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCreditInfoes_FromUserId",
                table: "UserCreditInfoes",
                column: "FromUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCreditInfoes_UserInfoes_FromUserId",
                table: "UserCreditInfoes",
                column: "FromUserId",
                principalTable: "UserInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCreditInfoes_UserInfoes_ToUserId",
                table: "UserCreditInfoes",
                column: "ToUserId",
                principalTable: "UserInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCreditInfoes_UserInfoes_FromUserId",
                table: "UserCreditInfoes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCreditInfoes_UserInfoes_ToUserId",
                table: "UserCreditInfoes");

            migrationBuilder.DropIndex(
                name: "IX_UserInfoes_TelegramUserId",
                table: "UserInfoes");

            migrationBuilder.DropIndex(
                name: "IX_UserCreditInfoes_FromUserId",
                table: "UserCreditInfoes");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "UserInfoes");

            migrationBuilder.DropColumn(
                name: "RoamUploadSize",
                table: "UserInfoes");

            migrationBuilder.DropColumn(
                name: "StaticUploadSize",
                table: "UserInfoes");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "UserCreditInfoes");

            migrationBuilder.DropColumn(
                name: "FromUserId",
                table: "UserCreditInfoes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "UserCreditInfoes");

            migrationBuilder.RenameColumn(
                name: "TelegramUserId",
                table: "UserInfoes",
                newName: "UserCreditId");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "UserCreditInfoes",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCreditInfoes_ToUserId",
                table: "UserCreditInfoes",
                newName: "IX_UserCreditInfoes_UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "Password",
                table: "UserInfoes",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "Credit",
                table: "UserCreditInfoes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "RoamUploadSize",
                table: "UserCreditInfoes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StaticUploadSize",
                table: "UserCreditInfoes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfoes_UserCreditId",
                table: "UserInfoes",
                column: "UserCreditId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCreditInfoes_UserInfoes_UserId",
                table: "UserCreditInfoes",
                column: "UserId",
                principalTable: "UserInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfoes_UserCreditInfoes_UserCreditId",
                table: "UserInfoes",
                column: "UserCreditId",
                principalTable: "UserCreditInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
