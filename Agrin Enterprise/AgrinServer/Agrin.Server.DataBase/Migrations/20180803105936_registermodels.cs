using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agrin.Server.DataBase.Migrations
{
    public partial class registermodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExceptionInfoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HttpErrorCode = table.Column<int>(nullable: true),
                    ErrorCode = table.Column<int>(nullable: true),
                    ExceptionType = table.Column<string>(nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    HelpUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptionInfoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestExceptionInfoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Mesage = table.Column<string>(nullable: true),
                    HttpErrorCode = table.Column<int>(nullable: true),
                    ExceptionType = table.Column<string>(nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    StackTrace = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestExceptionInfoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestExceptionInfoes_UserInfoes_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserConfirmHashInfoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    RandomNumber = table.Column<int>(nullable: false),
                    RandomGuid = table.Column<Guid>(nullable: false),
                    IsUsed = table.Column<bool>(nullable: false),
                    CreatedDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConfirmHashInfoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConfirmHashInfoes_UserInfoes_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    RequestIdeaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentInfo_RequestExceptionInfoes_RequestIdeaId",
                        column: x => x.RequestIdeaId,
                        principalTable: "RequestExceptionInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentInfo_UserInfoes_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentInfo_RequestIdeaId",
                table: "CommentInfo",
                column: "RequestIdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentInfo_UserId",
                table: "CommentInfo",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestExceptionInfoes_UserId",
                table: "RequestExceptionInfoes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserConfirmHashInfoes_UserId",
                table: "UserConfirmHashInfoes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentInfo");

            migrationBuilder.DropTable(
                name: "ExceptionInfoes");

            migrationBuilder.DropTable(
                name: "UserConfirmHashInfoes");

            migrationBuilder.DropTable(
                name: "RequestExceptionInfoes");
        }
    }
}
