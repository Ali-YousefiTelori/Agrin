using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agrin.Server.DataBase.Migrations
{
    public partial class newData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCategoryTagRelationInfoes_PostCategoryTagInfoes_TagId",
                table: "PostCategoryTagRelationInfoes");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTagRelationInfoes_PostCategoryTagInfoes_TagId",
                table: "PostTagRelationInfoes");

            migrationBuilder.DropTable(
                name: "FileInfo");

            migrationBuilder.DropTable(
                name: "PostCategoryTagInfoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostTagRelationInfoes",
                table: "PostTagRelationInfoes");

            migrationBuilder.DropIndex(
                name: "IX_PostTagRelationInfoes_TagId",
                table: "PostTagRelationInfoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostCategoryTagRelationInfoes",
                table: "PostCategoryTagRelationInfoes");

            migrationBuilder.DropIndex(
                name: "IX_PostCategoryTagRelationInfoes_TagId",
                table: "PostCategoryTagRelationInfoes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PostTagRelationInfoes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PostCategoryTagRelationInfoes");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "UserInfoes",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserCreditId",
                table: "UserInfoes",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostTagRelationInfoes",
                table: "PostTagRelationInfoes",
                columns: new[] { "TagId", "PostId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostCategoryTagRelationInfoes",
                table: "PostCategoryTagRelationInfoes",
                columns: new[] { "TagId", "PostCategoryId" });

            migrationBuilder.CreateTable(
                name: "DirectFileInfoes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServerName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectFileInfoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectFolderInfoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    ParentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectFolderInfoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectFolderInfoes_DirectFolderInfoes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "DirectFolderInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostFileInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServerAddress = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true),
                    VersionNumber = table.Column<int>(nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    OperationSystemSupports = table.Column<int>(nullable: false),
                    PostId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostFileInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostFileInfo_PostInfoes_PostId",
                        column: x => x.PostId,
                        principalTable: "PostInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagInfoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagInfoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCreditInfoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    StaticUploadSize = table.Column<long>(nullable: false),
                    RoamUploadSize = table.Column<long>(nullable: false),
                    Credit = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCreditInfoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCreditInfoes_UserInfoes_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessionInfoes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstKey = table.Column<Guid>(nullable: false),
                    SecondKey = table.Column<Guid>(nullable: false),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    OsName = table.Column<string>(nullable: true),
                    OsVersionNumber = table.Column<string>(nullable: true),
                    OsVersionName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessionInfoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessionInfoes_UserInfoes_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectFileToUserRelationInfoes",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    DirectFileId = table.Column<long>(nullable: false),
                    AccessType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectFileToUserRelationInfoes", x => new { x.UserId, x.DirectFileId });
                    table.ForeignKey(
                        name: "FK_DirectFileToUserRelationInfoes_DirectFileInfoes_DirectFileId",
                        column: x => x.DirectFileId,
                        principalTable: "DirectFileInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectFileToUserRelationInfoes_UserInfoes_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectFolderToUserRelationInfoes",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    DirectFolderId = table.Column<int>(nullable: false),
                    AccessType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectFolderToUserRelationInfoes", x => new { x.UserId, x.DirectFolderId });
                    table.ForeignKey(
                        name: "FK_DirectFolderToUserRelationInfoes_DirectFolderInfoes_DirectFolderId",
                        column: x => x.DirectFolderId,
                        principalTable: "DirectFolderInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectFolderToUserRelationInfoes_UserInfoes_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInfoes_UserCreditId",
                table: "UserInfoes",
                column: "UserCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfoes_UserName",
                table: "UserInfoes",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DirectFileToUserRelationInfoes_DirectFileId",
                table: "DirectFileToUserRelationInfoes",
                column: "DirectFileId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectFolderInfoes_Name",
                table: "DirectFolderInfoes",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DirectFolderInfoes_ParentId",
                table: "DirectFolderInfoes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectFolderToUserRelationInfoes_DirectFolderId",
                table: "DirectFolderToUserRelationInfoes",
                column: "DirectFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_PostFileInfo_PostId",
                table: "PostFileInfo",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCreditInfoes_UserId",
                table: "UserCreditInfoes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessionInfoes_UserId",
                table: "UserSessionInfoes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategoryTagRelationInfoes_TagInfoes_TagId",
                table: "PostCategoryTagRelationInfoes",
                column: "TagId",
                principalTable: "TagInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTagRelationInfoes_TagInfoes_TagId",
                table: "PostTagRelationInfoes",
                column: "TagId",
                principalTable: "TagInfoes",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCategoryTagRelationInfoes_TagInfoes_TagId",
                table: "PostCategoryTagRelationInfoes");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTagRelationInfoes_TagInfoes_TagId",
                table: "PostTagRelationInfoes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInfoes_UserCreditInfoes_UserCreditId",
                table: "UserInfoes");

            migrationBuilder.DropTable(
                name: "DirectFileToUserRelationInfoes");

            migrationBuilder.DropTable(
                name: "DirectFolderToUserRelationInfoes");

            migrationBuilder.DropTable(
                name: "PostFileInfo");

            migrationBuilder.DropTable(
                name: "TagInfoes");

            migrationBuilder.DropTable(
                name: "UserCreditInfoes");

            migrationBuilder.DropTable(
                name: "UserSessionInfoes");

            migrationBuilder.DropTable(
                name: "DirectFileInfoes");

            migrationBuilder.DropTable(
                name: "DirectFolderInfoes");

            migrationBuilder.DropIndex(
                name: "IX_UserInfoes_UserCreditId",
                table: "UserInfoes");

            migrationBuilder.DropIndex(
                name: "IX_UserInfoes_UserName",
                table: "UserInfoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostTagRelationInfoes",
                table: "PostTagRelationInfoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostCategoryTagRelationInfoes",
                table: "PostCategoryTagRelationInfoes");

            migrationBuilder.DropColumn(
                name: "UserCreditId",
                table: "UserInfoes");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "UserInfoes",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PostTagRelationInfoes",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PostCategoryTagRelationInfoes",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostTagRelationInfoes",
                table: "PostTagRelationInfoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostCategoryTagRelationInfoes",
                table: "PostCategoryTagRelationInfoes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FileInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(nullable: true),
                    PostId = table.Column<int>(nullable: false),
                    ServerAddress = table.Column<string>(nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    Version = table.Column<string>(nullable: true),
                    VersionNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileInfo_PostInfoes_PostId",
                        column: x => x.PostId,
                        principalTable: "PostInfoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostCategoryTagInfoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategoryTagInfoes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostTagRelationInfoes_TagId",
                table: "PostTagRelationInfoes",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_PostCategoryTagRelationInfoes_TagId",
                table: "PostCategoryTagRelationInfoes",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_FileInfo_PostId",
                table: "FileInfo",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategoryTagRelationInfoes_PostCategoryTagInfoes_TagId",
                table: "PostCategoryTagRelationInfoes",
                column: "TagId",
                principalTable: "PostCategoryTagInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTagRelationInfoes_PostCategoryTagInfoes_TagId",
                table: "PostTagRelationInfoes",
                column: "TagId",
                principalTable: "PostCategoryTagInfoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
