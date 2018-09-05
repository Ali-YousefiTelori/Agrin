using Microsoft.EntityFrameworkCore.Migrations;

namespace Agrin.Server.DataBase.Migrations
{
    public partial class idea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentInfo_RequestExceptionInfoes_RequestIdeaId",
                table: "CommentInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestExceptionInfoes_UserInfoes_UserId",
                table: "RequestExceptionInfoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestExceptionInfoes",
                table: "RequestExceptionInfoes");

            migrationBuilder.RenameTable(
                name: "RequestExceptionInfoes",
                newName: "RequestIdeaInfoes");

            migrationBuilder.RenameIndex(
                name: "IX_RequestExceptionInfoes_UserId",
                table: "RequestIdeaInfoes",
                newName: "IX_RequestIdeaInfoes_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestIdeaInfoes",
                table: "RequestIdeaInfoes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentInfo_RequestIdeaInfoes_RequestIdeaId",
                table: "CommentInfo",
                column: "RequestIdeaId",
                principalTable: "RequestIdeaInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestIdeaInfoes_UserInfoes_UserId",
                table: "RequestIdeaInfoes",
                column: "UserId",
                principalTable: "UserInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentInfo_RequestIdeaInfoes_RequestIdeaId",
                table: "CommentInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestIdeaInfoes_UserInfoes_UserId",
                table: "RequestIdeaInfoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestIdeaInfoes",
                table: "RequestIdeaInfoes");

            migrationBuilder.RenameTable(
                name: "RequestIdeaInfoes",
                newName: "RequestExceptionInfoes");

            migrationBuilder.RenameIndex(
                name: "IX_RequestIdeaInfoes_UserId",
                table: "RequestExceptionInfoes",
                newName: "IX_RequestExceptionInfoes_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestExceptionInfoes",
                table: "RequestExceptionInfoes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentInfo_RequestExceptionInfoes_RequestIdeaId",
                table: "CommentInfo",
                column: "RequestIdeaId",
                principalTable: "RequestExceptionInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestExceptionInfoes_UserInfoes_UserId",
                table: "RequestExceptionInfoes",
                column: "UserId",
                principalTable: "UserInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
