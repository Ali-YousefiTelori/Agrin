using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agrin.Server.DataBase.Migrations
{
    public partial class likesandcomments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentInfo_RequestIdeaInfoes_RequestIdeaId",
                table: "CommentInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentInfo_UserInfoes_UserId",
                table: "CommentInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentInfo",
                table: "CommentInfo");

            migrationBuilder.RenameTable(
                name: "CommentInfo",
                newName: "CommentInfoes");

            migrationBuilder.RenameIndex(
                name: "IX_CommentInfo_UserId",
                table: "CommentInfoes",
                newName: "IX_CommentInfoes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentInfo_RequestIdeaId",
                table: "CommentInfoes",
                newName: "IX_CommentInfoes_RequestIdeaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentInfoes",
                table: "CommentInfoes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LikeInfoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    RequestIdeaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeInfoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LikeInfoes_RequestIdeaInfoes_RequestIdeaId",
                        column: x => x.RequestIdeaId,
                        principalTable: "RequestIdeaInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LikeInfoes_UserInfoes_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikeInfoes_RequestIdeaId",
                table: "LikeInfoes",
                column: "RequestIdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_LikeInfoes_UserId",
                table: "LikeInfoes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentInfoes_RequestIdeaInfoes_RequestIdeaId",
                table: "CommentInfoes",
                column: "RequestIdeaId",
                principalTable: "RequestIdeaInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentInfoes_UserInfoes_UserId",
                table: "CommentInfoes",
                column: "UserId",
                principalTable: "UserInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentInfoes_RequestIdeaInfoes_RequestIdeaId",
                table: "CommentInfoes");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentInfoes_UserInfoes_UserId",
                table: "CommentInfoes");

            migrationBuilder.DropTable(
                name: "LikeInfoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentInfoes",
                table: "CommentInfoes");

            migrationBuilder.RenameTable(
                name: "CommentInfoes",
                newName: "CommentInfo");

            migrationBuilder.RenameIndex(
                name: "IX_CommentInfoes_UserId",
                table: "CommentInfo",
                newName: "IX_CommentInfo_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentInfoes_RequestIdeaId",
                table: "CommentInfo",
                newName: "IX_CommentInfo_RequestIdeaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentInfo",
                table: "CommentInfo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentInfo_RequestIdeaInfoes_RequestIdeaId",
                table: "CommentInfo",
                column: "RequestIdeaId",
                principalTable: "RequestIdeaInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentInfo_UserInfoes_UserId",
                table: "CommentInfo",
                column: "UserId",
                principalTable: "UserInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
