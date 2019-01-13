using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agrin.Server.DataBase.Migrations
{
    public partial class newupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentInfoes_RequestIdeaInfoes_RequestIdeaId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentInfoes_UserInfoes_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectFileInfoes_ServerInfoes_ServerId",
                table: "DirectFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectFileToUserRelationInfoes_DirectFileInfoes_DirectFileId",
                table: "DirectFileToUserRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectFileToUserRelationInfoes_UserInfoes_UserId",
                table: "DirectFileToUserRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectFolderInfoes_DirectFolderInfoes_ParentId",
                table: "DirectFolders");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectFolderToUserRelationInfoes_DirectFolderInfoes_DirectFolderId",
                table: "DirectFolderToUserRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectFolderToUserRelationInfoes_UserInfoes_UserId",
                table: "DirectFolderToUserRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_FileInfoes_PostInfoes_PostId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_FileInfoes_ServerInfoes_ServerId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_FileInfoes_VisitCardInfoes_VisitCardInfoId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_LanguageKeyInfoes_LanguageInfoes_LanguageId",
                table: "LanguageKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_LikeInfoes_RequestIdeaInfoes_RequestIdeaId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_LikeInfoes_UserInfoes_UserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_PostCategoryInfoes_LanguageKeyInfoes_LanguageKeyId",
                table: "PostCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PostInfoes_PostCategoryInfoes_CategoryId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_PostInfoes_PostSoundInfoes_PostMusicId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_PostInfoes_PostVideoInfoes_PostVideoId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_PostInfoes_UserInfoes_UserId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_PostSubCategoryInfoes_LanguageKeyInfoes_LanguageKeyId",
                table: "PostSubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PostSubCategoryInfoes_PostCategoryInfoes_PostCategoryId",
                table: "PostSubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTagRelationInfoes_PostInfoes_PostId",
                table: "PostTagRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTagRelationInfoes_TagInfoes_TagId",
                table: "PostTagRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestIdeaInfoes_UserInfoes_UserId",
                table: "RequestIdeas");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConfirmHashInfoes_UserInfoes_UserId",
                table: "UserConfirmHashes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCreditInfoes_UserInfoes_FromUserId",
                table: "UserCredits");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCreditInfoes_UserInfoes_ToUserId",
                table: "UserCredits");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleInfoes_UserInfoes_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionInfoes_UserInfoes_UserId",
                table: "UserSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitCardInfoes_UserInfoes_UserId",
                table: "VisitCards");

            migrationBuilder.DropTable(
                name: "PostCategorySubCategoryRelations");

            migrationBuilder.DropIndex(
                name: "IX_UserInfoes_UserName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_DirectFolderInfoes_Name",
                table: "DirectFolders");

            migrationBuilder.RenameIndex(
                name: "IX_VisitCardInfoes_UserId",
                table: "VisitCards",
                newName: "IX_VisitCards_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessionInfoes_UserId",
                table: "UserSessions",
                newName: "IX_UserSessions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserInfoes_TelegramUserId",
                table: "Users",
                newName: "IX_Users_TelegramUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoleInfoes_UserId",
                table: "UserRoles",
                newName: "IX_UserRoles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCreditInfoes_ToUserId",
                table: "UserCredits",
                newName: "IX_UserCredits_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCreditInfoes_Key",
                table: "UserCredits",
                newName: "IX_UserCredits_Key");

            migrationBuilder.RenameIndex(
                name: "IX_UserCreditInfoes_FromUserId",
                table: "UserCredits",
                newName: "IX_UserCredits_FromUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserConfirmHashInfoes_UserId",
                table: "UserConfirmHashes",
                newName: "IX_UserConfirmHashes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestIdeaInfoes_UserId",
                table: "RequestIdeas",
                newName: "IX_RequestIdeas_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PostTagRelationInfoes_PostId",
                table: "PostTagRelations",
                newName: "IX_PostTagRelations_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_PostSubCategoryInfoes_PostCategoryId",
                table: "PostSubCategories",
                newName: "IX_PostSubCategories_PostCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_PostSubCategoryInfoes_LanguageKeyId",
                table: "PostSubCategories",
                newName: "IX_PostSubCategories_LanguageKeyId");

            migrationBuilder.RenameIndex(
                name: "IX_PostInfoes_UserId",
                table: "Posts",
                newName: "IX_Posts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PostInfoes_PostVideoId",
                table: "Posts",
                newName: "IX_Posts_PostVideoId");

            migrationBuilder.RenameIndex(
                name: "IX_PostInfoes_PostMusicId",
                table: "Posts",
                newName: "IX_Posts_PostMusicId");

            migrationBuilder.RenameIndex(
                name: "IX_PostInfoes_CategoryId",
                table: "Posts",
                newName: "IX_Posts_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_PostCategoryInfoes_LanguageKeyId",
                table: "PostCategories",
                newName: "IX_PostCategories_LanguageKeyId");

            migrationBuilder.RenameIndex(
                name: "IX_LikeInfoes_UserId",
                table: "Likes",
                newName: "IX_Likes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LikeInfoes_RequestIdeaId",
                table: "Likes",
                newName: "IX_Likes_RequestIdeaId");

            migrationBuilder.RenameIndex(
                name: "IX_LanguageKeyInfoes_LanguageId",
                table: "LanguageKeys",
                newName: "IX_LanguageKeys_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_FileInfoes_VisitCardInfoId",
                table: "Files",
                newName: "IX_Files_VisitCardInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_FileInfoes_ServerId",
                table: "Files",
                newName: "IX_Files_ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_FileInfoes_PostId",
                table: "Files",
                newName: "IX_Files_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_DirectFolderToUserRelationInfoes_DirectFolderId",
                table: "DirectFolderToUserRelations",
                newName: "IX_DirectFolderToUserRelations_DirectFolderId");

            migrationBuilder.RenameIndex(
                name: "IX_DirectFolderInfoes_ParentId",
                table: "DirectFolders",
                newName: "IX_DirectFolders_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_DirectFileToUserRelationInfoes_DirectFileId",
                table: "DirectFileToUserRelations",
                newName: "IX_DirectFileToUserRelations_DirectFileId");

            migrationBuilder.RenameIndex(
                name: "IX_DirectFileInfoes_ServerId",
                table: "DirectFiles",
                newName: "IX_DirectFiles_ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentInfoes_UserId",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentInfoes_RequestIdeaId",
                table: "Comments",
                newName: "IX_Comments_RequestIdeaId");

            migrationBuilder.AlterColumn<long>(
                name: "StaticUploadSize",
                table: "Users",
                nullable: false,
                oldClrType: typeof(long),
                oldDefaultValueSql: "(CONVERT([bigint],(0)))");

            migrationBuilder.AlterColumn<long>(
                name: "RoamUploadSize",
                table: "Users",
                nullable: false,
                oldClrType: typeof(long),
                oldDefaultValueSql: "(CONVERT([bigint],(0)))");

            migrationBuilder.AlterColumn<long>(
                name: "Credit",
                table: "Users",
                nullable: false,
                oldClrType: typeof(long),
                oldDefaultValueSql: "(CONVERT([bigint],(0)))");

            migrationBuilder.AlterColumn<byte>(
                name: "Type",
                table: "UserCredits",
                nullable: false,
                oldClrType: typeof(byte),
                oldDefaultValueSql: "(CONVERT([tinyint],(0)))");

            migrationBuilder.AddColumn<int>(
                name: "RequestIdeaId",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestIdeaInfoId",
                table: "Files",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDateTime",
                table: "DirectFiles",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "('0001-01-01T00:00:00.0000000')");

            migrationBuilder.CreateTable(
                name: "PostCategorySubCategoryRelationInfo",
                columns: table => new
                {
                    PostCategoryId = table.Column<int>(nullable: false),
                    PostSubCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategorySubCategoryRelationInfo", x => new { x.PostSubCategoryId, x.PostCategoryId });
                    table.ForeignKey(
                        name: "FK_PostCategorySubCategoryRelationInfo_PostCategories_PostCategoryId",
                        column: x => x.PostCategoryId,
                        principalTable: "PostCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostCategorySubCategoryRelationInfo_PostSubCategories_PostSubCategoryId",
                        column: x => x.PostSubCategoryId,
                        principalTable: "PostSubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_RequestIdeaInfoId",
                table: "Files",
                column: "RequestIdeaInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectFolders_Name",
                table: "DirectFolders",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PostCategorySubCategoryRelationInfo_PostCategoryId",
                table: "PostCategorySubCategoryRelationInfo",
                column: "PostCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_RequestIdeas_RequestIdeaId",
                table: "Comments",
                column: "RequestIdeaId",
                principalTable: "RequestIdeas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFiles_Servers_ServerId",
                table: "DirectFiles",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFileToUserRelations_DirectFiles_DirectFileId",
                table: "DirectFileToUserRelations",
                column: "DirectFileId",
                principalTable: "DirectFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFileToUserRelations_Users_UserId",
                table: "DirectFileToUserRelations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFolders_DirectFolders_ParentId",
                table: "DirectFolders",
                column: "ParentId",
                principalTable: "DirectFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFolderToUserRelations_DirectFolders_DirectFolderId",
                table: "DirectFolderToUserRelations",
                column: "DirectFolderId",
                principalTable: "DirectFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFolderToUserRelations_Users_UserId",
                table: "DirectFolderToUserRelations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Posts_PostId",
                table: "Files",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_RequestIdeas_RequestIdeaInfoId",
                table: "Files",
                column: "RequestIdeaInfoId",
                principalTable: "RequestIdeas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Servers_ServerId",
                table: "Files",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_VisitCards_VisitCardInfoId",
                table: "Files",
                column: "VisitCardInfoId",
                principalTable: "VisitCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LanguageKeys_Languages_LanguageId",
                table: "LanguageKeys",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_RequestIdeas_RequestIdeaId",
                table: "Likes",
                column: "RequestIdeaId",
                principalTable: "RequestIdeas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_UserId",
                table: "Likes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategories_LanguageKeys_LanguageKeyId",
                table: "PostCategories",
                column: "LanguageKeyId",
                principalTable: "LanguageKeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_PostCategories_CategoryId",
                table: "Posts",
                column: "CategoryId",
                principalTable: "PostCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_PostSounds_PostMusicId",
                table: "Posts",
                column: "PostMusicId",
                principalTable: "PostSounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_PostVideoes_PostVideoId",
                table: "Posts",
                column: "PostVideoId",
                principalTable: "PostVideoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostSubCategories_LanguageKeys_LanguageKeyId",
                table: "PostSubCategories",
                column: "LanguageKeyId",
                principalTable: "LanguageKeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostSubCategories_PostCategories_PostCategoryId",
                table: "PostSubCategories",
                column: "PostCategoryId",
                principalTable: "PostCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTagRelations_Posts_PostId",
                table: "PostTagRelations",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTagRelations_Tags_TagId",
                table: "PostTagRelations",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestIdeas_Users_UserId",
                table: "RequestIdeas",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserConfirmHashes_Users_UserId",
                table: "UserConfirmHashes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCredits_Users_FromUserId",
                table: "UserCredits",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCredits_Users_ToUserId",
                table: "UserCredits",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessions_Users_UserId",
                table: "UserSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitCards_Users_UserId",
                table: "VisitCards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_RequestIdeas_RequestIdeaId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectFiles_Servers_ServerId",
                table: "DirectFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectFileToUserRelations_DirectFiles_DirectFileId",
                table: "DirectFileToUserRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectFileToUserRelations_Users_UserId",
                table: "DirectFileToUserRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectFolders_DirectFolders_ParentId",
                table: "DirectFolders");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectFolderToUserRelations_DirectFolders_DirectFolderId",
                table: "DirectFolderToUserRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectFolderToUserRelations_Users_UserId",
                table: "DirectFolderToUserRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Posts_PostId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_RequestIdeas_RequestIdeaInfoId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Servers_ServerId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_VisitCards_VisitCardInfoId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_LanguageKeys_Languages_LanguageId",
                table: "LanguageKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_RequestIdeas_RequestIdeaId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Users_UserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_PostCategories_LanguageKeys_LanguageKeyId",
                table: "PostCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_PostCategories_CategoryId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_PostSounds_PostMusicId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_PostVideoes_PostVideoId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_PostSubCategories_LanguageKeys_LanguageKeyId",
                table: "PostSubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PostSubCategories_PostCategories_PostCategoryId",
                table: "PostSubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTagRelations_Posts_PostId",
                table: "PostTagRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTagRelations_Tags_TagId",
                table: "PostTagRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestIdeas_Users_UserId",
                table: "RequestIdeas");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConfirmHashes_Users_UserId",
                table: "UserConfirmHashes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCredits_Users_FromUserId",
                table: "UserCredits");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCredits_Users_ToUserId",
                table: "UserCredits");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessions_Users_UserId",
                table: "UserSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitCards_Users_UserId",
                table: "VisitCards");

            migrationBuilder.DropTable(
                name: "PostCategorySubCategoryRelationInfo");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Files_RequestIdeaInfoId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_DirectFolders_Name",
                table: "DirectFolders");

            migrationBuilder.DropColumn(
                name: "RequestIdeaId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "RequestIdeaInfoId",
                table: "Files");

            migrationBuilder.RenameIndex(
                name: "IX_VisitCards_UserId",
                table: "VisitCards",
                newName: "IX_VisitCardInfoes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                newName: "IX_UserSessionInfoes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_TelegramUserId",
                table: "Users",
                newName: "IX_UserInfoes_TelegramUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                newName: "IX_UserRoleInfoes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCredits_ToUserId",
                table: "UserCredits",
                newName: "IX_UserCreditInfoes_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCredits_Key",
                table: "UserCredits",
                newName: "IX_UserCreditInfoes_Key");

            migrationBuilder.RenameIndex(
                name: "IX_UserCredits_FromUserId",
                table: "UserCredits",
                newName: "IX_UserCreditInfoes_FromUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserConfirmHashes_UserId",
                table: "UserConfirmHashes",
                newName: "IX_UserConfirmHashInfoes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestIdeas_UserId",
                table: "RequestIdeas",
                newName: "IX_RequestIdeaInfoes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PostTagRelations_PostId",
                table: "PostTagRelations",
                newName: "IX_PostTagRelationInfoes_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_PostSubCategories_PostCategoryId",
                table: "PostSubCategories",
                newName: "IX_PostSubCategoryInfoes_PostCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_PostSubCategories_LanguageKeyId",
                table: "PostSubCategories",
                newName: "IX_PostSubCategoryInfoes_LanguageKeyId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                newName: "IX_PostInfoes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_PostVideoId",
                table: "Posts",
                newName: "IX_PostInfoes_PostVideoId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_PostMusicId",
                table: "Posts",
                newName: "IX_PostInfoes_PostMusicId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_CategoryId",
                table: "Posts",
                newName: "IX_PostInfoes_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_PostCategories_LanguageKeyId",
                table: "PostCategories",
                newName: "IX_PostCategoryInfoes_LanguageKeyId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                newName: "IX_LikeInfoes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_RequestIdeaId",
                table: "Likes",
                newName: "IX_LikeInfoes_RequestIdeaId");

            migrationBuilder.RenameIndex(
                name: "IX_LanguageKeys_LanguageId",
                table: "LanguageKeys",
                newName: "IX_LanguageKeyInfoes_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_VisitCardInfoId",
                table: "Files",
                newName: "IX_FileInfoes_VisitCardInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_ServerId",
                table: "Files",
                newName: "IX_FileInfoes_ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_PostId",
                table: "Files",
                newName: "IX_FileInfoes_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_DirectFolderToUserRelations_DirectFolderId",
                table: "DirectFolderToUserRelations",
                newName: "IX_DirectFolderToUserRelationInfoes_DirectFolderId");

            migrationBuilder.RenameIndex(
                name: "IX_DirectFolders_ParentId",
                table: "DirectFolders",
                newName: "IX_DirectFolderInfoes_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_DirectFileToUserRelations_DirectFileId",
                table: "DirectFileToUserRelations",
                newName: "IX_DirectFileToUserRelationInfoes_DirectFileId");

            migrationBuilder.RenameIndex(
                name: "IX_DirectFiles_ServerId",
                table: "DirectFiles",
                newName: "IX_DirectFileInfoes_ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                newName: "IX_CommentInfoes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_RequestIdeaId",
                table: "Comments",
                newName: "IX_CommentInfoes_RequestIdeaId");

            migrationBuilder.AlterColumn<long>(
                name: "StaticUploadSize",
                table: "Users",
                nullable: false,
                defaultValueSql: "(CONVERT([bigint],(0)))",
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "RoamUploadSize",
                table: "Users",
                nullable: false,
                defaultValueSql: "(CONVERT([bigint],(0)))",
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "Credit",
                table: "Users",
                nullable: false,
                defaultValueSql: "(CONVERT([bigint],(0)))",
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<byte>(
                name: "Type",
                table: "UserCredits",
                nullable: false,
                defaultValueSql: "(CONVERT([tinyint],(0)))",
                oldClrType: typeof(byte));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDateTime",
                table: "DirectFiles",
                nullable: false,
                defaultValueSql: "('0001-01-01T00:00:00.0000000')",
                oldClrType: typeof(DateTime));

            migrationBuilder.CreateTable(
                name: "PostCategorySubCategoryRelations",
                columns: table => new
                {
                    PostSubCategoryId = table.Column<int>(nullable: false),
                    PostCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategorySubCategoryRelations", x => new { x.PostSubCategoryId, x.PostCategoryId });
                    table.ForeignKey(
                        name: "FK_PostCategorySubCategoryRelationInfo_PostCategoryInfoes_PostCategoryId",
                        column: x => x.PostCategoryId,
                        principalTable: "PostCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostCategorySubCategoryRelationInfo_PostSubCategoryInfoes_PostSubCategoryId",
                        column: x => x.PostSubCategoryId,
                        principalTable: "PostSubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInfoes_UserName",
                table: "Users",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DirectFolderInfoes_Name",
                table: "DirectFolders",
                column: "Name",
                unique: true,
                filter: "([Name] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_PostCategorySubCategoryRelationInfo_PostCategoryId",
                table: "PostCategorySubCategoryRelations",
                column: "PostCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentInfoes_RequestIdeaInfoes_RequestIdeaId",
                table: "Comments",
                column: "RequestIdeaId",
                principalTable: "RequestIdeas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentInfoes_UserInfoes_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFileInfoes_ServerInfoes_ServerId",
                table: "DirectFiles",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFileToUserRelationInfoes_DirectFileInfoes_DirectFileId",
                table: "DirectFileToUserRelations",
                column: "DirectFileId",
                principalTable: "DirectFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFileToUserRelationInfoes_UserInfoes_UserId",
                table: "DirectFileToUserRelations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFolderInfoes_DirectFolderInfoes_ParentId",
                table: "DirectFolders",
                column: "ParentId",
                principalTable: "DirectFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFolderToUserRelationInfoes_DirectFolderInfoes_DirectFolderId",
                table: "DirectFolderToUserRelations",
                column: "DirectFolderId",
                principalTable: "DirectFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectFolderToUserRelationInfoes_UserInfoes_UserId",
                table: "DirectFolderToUserRelations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileInfoes_PostInfoes_PostId",
                table: "Files",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FileInfoes_ServerInfoes_ServerId",
                table: "Files",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileInfoes_VisitCardInfoes_VisitCardInfoId",
                table: "Files",
                column: "VisitCardInfoId",
                principalTable: "VisitCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LanguageKeyInfoes_LanguageInfoes_LanguageId",
                table: "LanguageKeys",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LikeInfoes_RequestIdeaInfoes_RequestIdeaId",
                table: "Likes",
                column: "RequestIdeaId",
                principalTable: "RequestIdeas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LikeInfoes_UserInfoes_UserId",
                table: "Likes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategoryInfoes_LanguageKeyInfoes_LanguageKeyId",
                table: "PostCategories",
                column: "LanguageKeyId",
                principalTable: "LanguageKeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostInfoes_PostCategoryInfoes_CategoryId",
                table: "Posts",
                column: "CategoryId",
                principalTable: "PostCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostInfoes_PostSoundInfoes_PostMusicId",
                table: "Posts",
                column: "PostMusicId",
                principalTable: "PostSounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostInfoes_PostVideoInfoes_PostVideoId",
                table: "Posts",
                column: "PostVideoId",
                principalTable: "PostVideoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostInfoes_UserInfoes_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostSubCategoryInfoes_LanguageKeyInfoes_LanguageKeyId",
                table: "PostSubCategories",
                column: "LanguageKeyId",
                principalTable: "LanguageKeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostSubCategoryInfoes_PostCategoryInfoes_PostCategoryId",
                table: "PostSubCategories",
                column: "PostCategoryId",
                principalTable: "PostCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTagRelationInfoes_PostInfoes_PostId",
                table: "PostTagRelations",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTagRelationInfoes_TagInfoes_TagId",
                table: "PostTagRelations",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestIdeaInfoes_UserInfoes_UserId",
                table: "RequestIdeas",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserConfirmHashInfoes_UserInfoes_UserId",
                table: "UserConfirmHashes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCreditInfoes_UserInfoes_FromUserId",
                table: "UserCredits",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCreditInfoes_UserInfoes_ToUserId",
                table: "UserCredits",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleInfoes_UserInfoes_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionInfoes_UserInfoes_UserId",
                table: "UserSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitCardInfoes_UserInfoes_UserId",
                table: "VisitCards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
