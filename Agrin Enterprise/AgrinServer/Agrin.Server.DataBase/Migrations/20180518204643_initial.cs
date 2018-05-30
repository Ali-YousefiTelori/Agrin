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
                name: "FileInfo");

            migrationBuilder.DropTable(
                name: "PostCategoryTagRelationInfoes");

            migrationBuilder.DropTable(
                name: "PostTagRelationInfoes");

            migrationBuilder.DropTable(
                name: "PostInfoes");

            migrationBuilder.DropTable(
                name: "PostCategoryTagInfoes");

            migrationBuilder.DropTable(
                name: "PostCategoryInfoes");

            migrationBuilder.DropTable(
                name: "PostSoundInfoes");

            migrationBuilder.DropTable(
                name: "PostVideoInfoes");

            migrationBuilder.DropTable(
                name: "UserInfoes");
        }
    }
}
