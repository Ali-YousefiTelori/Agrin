// <auto-generated />
using System;
using Agrin.Server.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Agrin.Server.DataBase.Migrations
{
    [DbContext(typeof(AgrinContext))]
    [Migration("20190113180150_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Comments", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<string>("Message");

                    b.Property<int?>("RequestIdeaId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("RequestIdeaId")
                        .HasName("IX_CommentInfoes_RequestIdeaId");

                    b.HasIndex("UserId")
                        .HasName("IX_CommentInfoes_UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.DirectFiles", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                    b.Property<bool>("IsComplete");

                    b.Property<int>("ServerId");

                    b.HasKey("Id");

                    b.HasIndex("ServerId")
                        .HasName("IX_DirectFileInfoes_ServerId");

                    b.ToTable("DirectFiles");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.DirectFileToUserRelations", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<long>("DirectFileId");

                    b.Property<byte>("AccessType");

                    b.HasKey("UserId", "DirectFileId");

                    b.HasIndex("DirectFileId")
                        .HasName("IX_DirectFileToUserRelationInfoes_DirectFileId");

                    b.ToTable("DirectFileToUserRelations");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.DirectFolders", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int?>("ParentId");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasName("IX_DirectFolderInfoes_Name")
                        .HasFilter("([Name] IS NOT NULL)");

                    b.HasIndex("ParentId")
                        .HasName("IX_DirectFolderInfoes_ParentId");

                    b.ToTable("DirectFolders");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.DirectFolderToUserRelations", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("DirectFolderId");

                    b.Property<byte>("AccessType");

                    b.HasKey("UserId", "DirectFolderId");

                    b.HasIndex("DirectFolderId")
                        .HasName("IX_DirectFolderToUserRelationInfoes_DirectFolderId");

                    b.ToTable("DirectFolderToUserRelations");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Exceptions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ErrorCode");

                    b.Property<string>("ErrorMessage");

                    b.Property<string>("ExceptionType");

                    b.Property<string>("HelpUrl");

                    b.Property<int?>("HttpErrorCode");

                    b.HasKey("Id");

                    b.ToTable("Exceptions");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Files", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("PostId");

                    b.Property<int>("ServerId");

                    b.Property<byte>("Type");

                    b.Property<string>("Version")
                        .HasMaxLength(20);

                    b.Property<int?>("VersionNumber");

                    b.Property<int?>("VisitCardId");

                    b.Property<int?>("VisitCardInfoId");

                    b.HasKey("Id");

                    b.HasIndex("PostId")
                        .HasName("IX_FileInfoes_PostId");

                    b.HasIndex("ServerId")
                        .HasName("IX_FileInfoes_ServerId");

                    b.HasIndex("VisitCardInfoId")
                        .HasName("IX_FileInfoes_VisitCardInfoId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.LanguageKeys", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .HasMaxLength(50);

                    b.Property<string>("Key")
                        .HasMaxLength(20);

                    b.Property<int>("LanguageId");

                    b.HasKey("Id");

                    b.HasIndex("LanguageId")
                        .HasName("IX_LanguageKeyInfoes_LanguageId");

                    b.ToTable("LanguageKeys");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Languages", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Likes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("RequestIdeaId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("RequestIdeaId")
                        .HasName("IX_LikeInfoes_RequestIdeaId");

                    b.HasIndex("UserId")
                        .HasName("IX_LikeInfoes_UserId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.PostCategories", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("LanguageKeyId");

                    b.HasKey("Id");

                    b.HasIndex("LanguageKeyId")
                        .HasName("IX_PostCategoryInfoes_LanguageKeyId");

                    b.ToTable("PostCategories");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.PostCategorySubCategoryRelations", b =>
                {
                    b.Property<int>("PostSubCategoryId");

                    b.Property<int>("PostCategoryId");

                    b.HasKey("PostSubCategoryId", "PostCategoryId");

                    b.HasIndex("PostCategoryId")
                        .HasName("IX_PostCategorySubCategoryRelationInfo_PostCategoryId");

                    b.ToTable("PostCategorySubCategoryRelations");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Posts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId");

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<DateTime>("LastUpdateDateTime");

                    b.Property<DateTime>("LastUpdateFileVersionDateTime");

                    b.Property<int?>("PostMusicId");

                    b.Property<byte>("PostType");

                    b.Property<int?>("PostVideoId");

                    b.Property<int>("UserId");

                    b.Property<long>("ViewCount");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId")
                        .HasName("IX_PostInfoes_CategoryId");

                    b.HasIndex("PostMusicId")
                        .HasName("IX_PostInfoes_PostMusicId");

                    b.HasIndex("PostVideoId")
                        .HasName("IX_PostInfoes_PostVideoId");

                    b.HasIndex("UserId")
                        .HasName("IX_PostInfoes_UserId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.PostSounds", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("PostSounds");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.PostSubCategories", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("LanguageKeyId");

                    b.Property<int>("PostCategoryId");

                    b.HasKey("Id");

                    b.HasIndex("LanguageKeyId")
                        .HasName("IX_PostSubCategoryInfoes_LanguageKeyId");

                    b.HasIndex("PostCategoryId")
                        .HasName("IX_PostSubCategoryInfoes_PostCategoryId");

                    b.ToTable("PostSubCategories");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.PostTagRelations", b =>
                {
                    b.Property<int>("TagId");

                    b.Property<int>("PostId");

                    b.HasKey("TagId", "PostId");

                    b.HasIndex("PostId")
                        .HasName("IX_PostTagRelationInfoes_PostId");

                    b.ToTable("PostTagRelations");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.PostVideoes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("PostVideoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.RequestIdeas", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<string>("ErrorMessage");

                    b.Property<string>("ExceptionType");

                    b.Property<int?>("HttpErrorCode");

                    b.Property<string>("Message");

                    b.Property<string>("StackTrace");

                    b.Property<byte>("Status");

                    b.Property<string>("Title");

                    b.Property<byte>("Type");

                    b.Property<DateTime>("UpdatedDateTime");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .HasName("IX_RequestIdeaInfoes_UserId");

                    b.ToTable("RequestIdeas");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Servers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Domain")
                        .HasMaxLength(50);

                    b.Property<string>("IpAddress")
                        .HasMaxLength(16);

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Tags", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.UserConfirmHashes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<bool>("IsUsed");

                    b.Property<Guid>("RandomGuid");

                    b.Property<int>("RandomNumber");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .HasName("IX_UserConfirmHashInfoes_UserId");

                    b.ToTable("UserConfirmHashes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.UserCredits", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Amount");

                    b.Property<int?>("FromUserId");

                    b.Property<Guid>("Key");

                    b.Property<int>("ToUserId");

                    b.Property<byte>("Type")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(CONVERT([tinyint],(0)))");

                    b.HasKey("Id");

                    b.HasIndex("FromUserId")
                        .HasName("IX_UserCreditInfoes_FromUserId");

                    b.HasIndex("Key")
                        .IsUnique()
                        .HasName("IX_UserCreditInfoes_Key");

                    b.HasIndex("ToUserId")
                        .HasName("IX_UserCreditInfoes_ToUserId");

                    b.ToTable("UserCredits");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.UserRoles", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte>("AccessType");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .HasName("IX_UserRoleInfoes_UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<long>("Credit")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(CONVERT([bigint],(0)))");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Family");

                    b.Property<string>("Name");

                    b.Property<long>("RoamUploadSize")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(CONVERT([bigint],(0)))");

                    b.Property<long>("StaticUploadSize")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(CONVERT([bigint],(0)))");

                    b.Property<byte>("Status");

                    b.Property<int?>("TelegramUserId");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("TelegramUserId")
                        .HasName("IX_UserInfoes_TelegramUserId");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasName("IX_UserInfoes_UserName")
                        .HasFilter("[UserName] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.UserSessions", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<string>("DeviceName")
                        .HasMaxLength(100);

                    b.Property<Guid>("FirstKey");

                    b.Property<bool>("IsActive");

                    b.Property<string>("OsName")
                        .HasMaxLength(50);

                    b.Property<string>("OsVersionName")
                        .HasMaxLength(50);

                    b.Property<string>("OsVersionNumber")
                        .HasMaxLength(15);

                    b.Property<Guid>("SecondKey");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .HasName("IX_UserSessionInfoes_UserId");

                    b.ToTable("UserSessions");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.VisitCards", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasMaxLength(350);

                    b.Property<string>("Title")
                        .HasMaxLength(100);

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .HasName("IX_VisitCardInfoes_UserId");

                    b.ToTable("VisitCards");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Comments", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.RequestIdeas", "RequestIdea")
                        .WithMany("Comments")
                        .HasForeignKey("RequestIdeaId")
                        .HasConstraintName("FK_CommentInfoes_RequestIdeaInfoes_RequestIdeaId");

                    b.HasOne("Agrin.Server.DataBase.Data.Users", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_CommentInfoes_UserInfoes_UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.DirectFiles", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.Servers", "Server")
                        .WithMany("DirectFiles")
                        .HasForeignKey("ServerId")
                        .HasConstraintName("FK_DirectFileInfoes_ServerInfoes_ServerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.DirectFileToUserRelations", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.DirectFiles", "DirectFile")
                        .WithMany("DirectFileToUserRelations")
                        .HasForeignKey("DirectFileId")
                        .HasConstraintName("FK_DirectFileToUserRelationInfoes_DirectFileInfoes_DirectFileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Agrin.Server.DataBase.Data.Users", "User")
                        .WithMany("DirectFileToUserRelations")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_DirectFileToUserRelationInfoes_UserInfoes_UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.DirectFolders", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.DirectFolders", "Parent")
                        .WithMany("InverseParent")
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK_DirectFolderInfoes_DirectFolderInfoes_ParentId");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.DirectFolderToUserRelations", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.DirectFolders", "DirectFolder")
                        .WithMany("DirectFolderToUserRelations")
                        .HasForeignKey("DirectFolderId")
                        .HasConstraintName("FK_DirectFolderToUserRelationInfoes_DirectFolderInfoes_DirectFolderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Agrin.Server.DataBase.Data.Users", "User")
                        .WithMany("DirectFolderToUserRelations")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_DirectFolderToUserRelationInfoes_UserInfoes_UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Files", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.Posts", "Post")
                        .WithMany("Files")
                        .HasForeignKey("PostId")
                        .HasConstraintName("FK_FileInfoes_PostInfoes_PostId");

                    b.HasOne("Agrin.Server.DataBase.Data.Servers", "Server")
                        .WithMany("Files")
                        .HasForeignKey("ServerId")
                        .HasConstraintName("FK_FileInfoes_ServerInfoes_ServerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Agrin.Server.DataBase.Data.VisitCards", "VisitCardInfo")
                        .WithMany("Files")
                        .HasForeignKey("VisitCardInfoId")
                        .HasConstraintName("FK_FileInfoes_VisitCardInfoes_VisitCardInfoId");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.LanguageKeys", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.Languages", "Language")
                        .WithMany("LanguageKeys")
                        .HasForeignKey("LanguageId")
                        .HasConstraintName("FK_LanguageKeyInfoes_LanguageInfoes_LanguageId");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Likes", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.RequestIdeas", "RequestIdea")
                        .WithMany("Likes")
                        .HasForeignKey("RequestIdeaId")
                        .HasConstraintName("FK_LikeInfoes_RequestIdeaInfoes_RequestIdeaId");

                    b.HasOne("Agrin.Server.DataBase.Data.Users", "User")
                        .WithMany("Likes")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_LikeInfoes_UserInfoes_UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.PostCategories", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.LanguageKeys", "LanguageKey")
                        .WithMany("PostCategories")
                        .HasForeignKey("LanguageKeyId")
                        .HasConstraintName("FK_PostCategoryInfoes_LanguageKeyInfoes_LanguageKeyId");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.PostCategorySubCategoryRelations", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.PostCategories", "PostCategory")
                        .WithMany("PostCategorySubCategoryRelations")
                        .HasForeignKey("PostCategoryId")
                        .HasConstraintName("FK_PostCategorySubCategoryRelationInfo_PostCategoryInfoes_PostCategoryId");

                    b.HasOne("Agrin.Server.DataBase.Data.PostSubCategories", "PostSubCategory")
                        .WithMany("PostCategorySubCategoryRelations")
                        .HasForeignKey("PostSubCategoryId")
                        .HasConstraintName("FK_PostCategorySubCategoryRelationInfo_PostSubCategoryInfoes_PostSubCategoryId");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.Posts", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.PostCategories", "Category")
                        .WithMany("Posts")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_PostInfoes_PostCategoryInfoes_CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Agrin.Server.DataBase.Data.PostSounds", "PostMusic")
                        .WithMany("Posts")
                        .HasForeignKey("PostMusicId")
                        .HasConstraintName("FK_PostInfoes_PostSoundInfoes_PostMusicId");

                    b.HasOne("Agrin.Server.DataBase.Data.PostVideoes", "PostVideo")
                        .WithMany("Posts")
                        .HasForeignKey("PostVideoId")
                        .HasConstraintName("FK_PostInfoes_PostVideoInfoes_PostVideoId");

                    b.HasOne("Agrin.Server.DataBase.Data.Users", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_PostInfoes_UserInfoes_UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.PostSubCategories", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.LanguageKeys", "LanguageKey")
                        .WithMany("PostSubCategories")
                        .HasForeignKey("LanguageKeyId")
                        .HasConstraintName("FK_PostSubCategoryInfoes_LanguageKeyInfoes_LanguageKeyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Agrin.Server.DataBase.Data.PostCategories", "PostCategory")
                        .WithMany("PostSubCategories")
                        .HasForeignKey("PostCategoryId")
                        .HasConstraintName("FK_PostSubCategoryInfoes_PostCategoryInfoes_PostCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.PostTagRelations", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.Posts", "Post")
                        .WithMany("PostTagRelations")
                        .HasForeignKey("PostId")
                        .HasConstraintName("FK_PostTagRelationInfoes_PostInfoes_PostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Agrin.Server.DataBase.Data.Tags", "Tag")
                        .WithMany("PostTagRelations")
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK_PostTagRelationInfoes_TagInfoes_TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.RequestIdeas", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.Users", "User")
                        .WithMany("RequestIdeas")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_RequestIdeaInfoes_UserInfoes_UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.UserConfirmHashes", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.Users", "User")
                        .WithMany("UserConfirmHashes")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserConfirmHashInfoes_UserInfoes_UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.UserCredits", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.Users", "FromUser")
                        .WithMany("UserCreditsFromUser")
                        .HasForeignKey("FromUserId")
                        .HasConstraintName("FK_UserCreditInfoes_UserInfoes_FromUserId");

                    b.HasOne("Agrin.Server.DataBase.Data.Users", "ToUser")
                        .WithMany("UserCreditsToUser")
                        .HasForeignKey("ToUserId")
                        .HasConstraintName("FK_UserCreditInfoes_UserInfoes_ToUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.UserRoles", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.Users", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserRoleInfoes_UserInfoes_UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.UserSessions", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.Users", "User")
                        .WithMany("UserSessions")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserSessionInfoes_UserInfoes_UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Data.VisitCards", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Data.Users", "User")
                        .WithMany("VisitCards")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_VisitCardInfoes_UserInfoes_UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
