﻿// <auto-generated />
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
    [Migration("20180901113345_renamefield")]
    partial class renamefield
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Agrin.Server.DataBase.Models.CommentInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<int?>("RequestIdeaId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("RequestIdeaId");

                    b.HasIndex("UserId");

                    b.ToTable("CommentInfo");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.DirectFileInfo", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<bool>("IsComplete");

                    b.Property<int>("ServerId");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("DirectFileInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.DirectFolderInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int?>("ParentId");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.HasIndex("ParentId");

                    b.ToTable("DirectFolderInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.ExceptionInfo", b =>
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

                    b.ToTable("ExceptionInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.PostCategoryInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("PostCategoryInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.PostFileInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OperationSystemSupports");

                    b.Property<int>("PostId");

                    b.Property<string>("ServerAddress");

                    b.Property<byte>("Type");

                    b.Property<string>("Version");

                    b.Property<int?>("VersionNumber");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("PostFileInfo");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.PostInfo", b =>
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

                    b.HasIndex("CategoryId");

                    b.HasIndex("PostMusicId");

                    b.HasIndex("PostVideoId");

                    b.HasIndex("UserId");

                    b.ToTable("PostInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.PostSoundInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("PostSoundInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.PostVideoInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("PostVideoInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.Relations.DirectFileToUserRelationInfo", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<long>("DirectFileId");

                    b.Property<byte>("AccessType");

                    b.HasKey("UserId", "DirectFileId");

                    b.HasIndex("DirectFileId");

                    b.ToTable("DirectFileToUserRelationInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.Relations.DirectFolderToUserRelationInfo", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("DirectFolderId");

                    b.Property<byte>("AccessType");

                    b.HasKey("UserId", "DirectFolderId");

                    b.HasIndex("DirectFolderId");

                    b.ToTable("DirectFolderToUserRelationInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.Relations.PostCategoryTagRelationInfo", b =>
                {
                    b.Property<int>("TagId");

                    b.Property<int>("PostCategoryId");

                    b.Property<bool>("IsHeaderTag");

                    b.HasKey("TagId", "PostCategoryId");

                    b.HasIndex("PostCategoryId");

                    b.ToTable("PostCategoryTagRelationInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.Relations.PostTagRelationInfo", b =>
                {
                    b.Property<int>("TagId");

                    b.Property<int>("PostId");

                    b.HasKey("TagId", "PostId");

                    b.HasIndex("PostId");

                    b.ToTable("PostTagRelationInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.RequestIdeaInfo", b =>
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

                    b.HasIndex("UserId");

                    b.ToTable("RequestIdeaInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.ServerInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Domain");

                    b.Property<string>("IpAddress");

                    b.HasKey("Id");

                    b.ToTable("ServerInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.TagInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("TagInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.UserConfirmHashInfo", b =>
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

                    b.HasIndex("UserId");

                    b.ToTable("UserConfirmHashInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.UserCreditInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Amount");

                    b.Property<int?>("FromUserId");

                    b.Property<Guid>("Key");

                    b.Property<int>("ToUserId");

                    b.Property<byte>("Type");

                    b.HasKey("Id");

                    b.HasIndex("FromUserId");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.HasIndex("ToUserId");

                    b.ToTable("UserCreditInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.UserInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<long>("Credit");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Family");

                    b.Property<string>("Name");

                    b.Property<long>("RoamUploadSize");

                    b.Property<long>("StaticUploadSize");

                    b.Property<byte>("Status");

                    b.Property<int?>("TelegramUserId");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("TelegramUserId")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("UserInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.UserSessionInfo", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<Guid>("FirstKey");

                    b.Property<bool>("IsActive");

                    b.Property<string>("OsName");

                    b.Property<string>("OsVersionName");

                    b.Property<string>("OsVersionNumber");

                    b.Property<Guid>("SecondKey");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserSessionInfoes");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.CommentInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.RequestIdeaInfo", "RequestIdeaInfo")
                        .WithMany("CommentInfoes")
                        .HasForeignKey("RequestIdeaId");

                    b.HasOne("Agrin.Server.DataBase.Models.UserInfo", "UserInfo")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.DirectFileInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.ServerInfo", "ServerInfo")
                        .WithMany("DirectFileInfoes")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.DirectFolderInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.DirectFolderInfo", "ParentFolderInfo")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.PostFileInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.PostInfo", "PostInfo")
                        .WithMany("FileInfoes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.PostInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.PostCategoryInfo", "PostCategoryInfo")
                        .WithMany("Posts")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Agrin.Server.DataBase.Models.PostSoundInfo", "PostMusicInfo")
                        .WithMany()
                        .HasForeignKey("PostMusicId");

                    b.HasOne("Agrin.Server.DataBase.Models.PostVideoInfo", "PostVideoInfo")
                        .WithMany()
                        .HasForeignKey("PostVideoId");

                    b.HasOne("Agrin.Server.DataBase.Models.UserInfo", "UserInfo")
                        .WithMany("PostInfoes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.Relations.DirectFileToUserRelationInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.DirectFileInfo", "DirectFileInfo")
                        .WithMany("DirectFileToUserRelationInfoes")
                        .HasForeignKey("DirectFileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Agrin.Server.DataBase.Models.UserInfo", "UserInfo")
                        .WithMany("DirectFileToUserRelationInfoes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.Relations.DirectFolderToUserRelationInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.DirectFolderInfo", "DirectFolderInfo")
                        .WithMany("DirectFolderToUserRelationInfoes")
                        .HasForeignKey("DirectFolderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Agrin.Server.DataBase.Models.UserInfo", "UserInfo")
                        .WithMany("DirectFolderToUserRelationInfoes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.Relations.PostCategoryTagRelationInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.PostCategoryInfo", "PostCategoryInfo")
                        .WithMany("PostCategoryTagRelationInfoes")
                        .HasForeignKey("PostCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Agrin.Server.DataBase.Models.TagInfo", "TagInfo")
                        .WithMany("PostCategoryTagRelationInfoes")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.Relations.PostTagRelationInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.PostInfo", "PostInfo")
                        .WithMany("PostTagRelationInfoes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Agrin.Server.DataBase.Models.TagInfo", "TagInfo")
                        .WithMany("PostTagRelationInfoes")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.RequestIdeaInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.UserInfo", "UserInfo")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.UserConfirmHashInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.UserInfo", "UserInfo")
                        .WithMany("UserConfirmHashInfoes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.UserCreditInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.UserInfo", "FromUserInfo")
                        .WithMany("FromUserCreditInfoes")
                        .HasForeignKey("FromUserId");

                    b.HasOne("Agrin.Server.DataBase.Models.UserInfo", "ToUserInfo")
                        .WithMany("ToUserCreditInfoes")
                        .HasForeignKey("ToUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Agrin.Server.DataBase.Models.UserSessionInfo", b =>
                {
                    b.HasOne("Agrin.Server.DataBase.Models.UserInfo", "UserInfo")
                        .WithMany("UserSessionInfoes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
