using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agrin.Server.DataBase.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "DirectFileToUserRelations");

            migrationBuilder.DropTable(
                name: "DirectFolderToUserRelations");

            migrationBuilder.DropTable(
                name: "Exceptions");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "PostCategorySubCategoryRelations");

            migrationBuilder.DropTable(
                name: "PostTagRelations");

            migrationBuilder.DropTable(
                name: "UserConfirmHashes");

            migrationBuilder.DropTable(
                name: "UserCredits");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "DirectFiles");

            migrationBuilder.DropTable(
                name: "DirectFolders");

            migrationBuilder.DropTable(
                name: "VisitCards");

            migrationBuilder.DropTable(
                name: "RequestIdeas");

            migrationBuilder.DropTable(
                name: "PostSubCategories");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "PostCategories");

            migrationBuilder.DropTable(
                name: "PostSounds");

            migrationBuilder.DropTable(
                name: "PostVideoes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "LanguageKeys");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
