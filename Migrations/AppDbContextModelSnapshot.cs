﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace server.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("CanvaDesign", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("CreatorId")
                        .HasColumnType("bigint");

                    b.Property<string>("DesignName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("DesignUrl")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("EmbedUrl")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Metadata")
                        .HasColumnType("longtext");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("ThumbnailUrl")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("CanvaDesigns", (string)null);
                });

            modelBuilder.Entity("Comment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("CommentText")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("varchar(1000)");

                    b.Property<long>("ContentId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ContentId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments", (string)null);
                });

            modelBuilder.Entity("Content", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.Property<long?>("MediaId")
                        .HasColumnType("bigint");

                    b.Property<string>("TextContent")
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("UploaderId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MediaId");

                    b.HasIndex("UploaderId");

                    b.HasIndex("EventId", "UploaderId", "Type")
                        .IsUnique();

                    b.ToTable("Contents", (string)null);
                });

            modelBuilder.Entity("Coupon", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<int>("Discount")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long?>("OwnerId")
                        .HasColumnType("bigint");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("TimesUsed")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UsageLimit")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Coupons", (string)null);
                });

            modelBuilder.Entity("Event", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("varchar(1000)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<long?>("OrganizerId")
                        .HasColumnType("bigint");

                    b.Property<string>("QRAlbum")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("QREvent")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("OrganizerId");

                    b.ToTable("Events", (string)null);
                });

            modelBuilder.Entity("EventSettings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.Property<string>("SettingKey")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("SettingValue")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("EventId")
                        .IsUnique();

                    b.ToTable("EventSettings", (string)null);
                });

            modelBuilder.Entity("Media", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("ApprovalStatus")
                        .HasColumnType("int");

                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("FileUrl")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Metadata")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("UploadedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("UploaderId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("UploaderId");

                    b.ToTable("MediaFiles", (string)null);
                });

            modelBuilder.Entity("Payment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(65,30)");

                    b.Property<long?>("CouponId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Metadata")
                        .HasColumnType("longtext");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PaymentReference")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CouponId");

                    b.HasIndex("UserId");

                    b.ToTable("Payments", (string)null);
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Plan")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ResetToken")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("ResetTokenExpiry")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("CanvaDesign", b =>
                {
                    b.HasOne("User", "Creator")
                        .WithMany("CanvaDesigns")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("Comment", b =>
                {
                    b.HasOne("Content", "Content")
                        .WithMany("Comments")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Content");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Content", b =>
                {
                    b.HasOne("Event", "Event")
                        .WithMany("Contents")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Media", "Media")
                        .WithMany()
                        .HasForeignKey("MediaId");

                    b.HasOne("User", "Uploader")
                        .WithMany("UploadedContents")
                        .HasForeignKey("UploaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Media");

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("Coupon", b =>
                {
                    b.HasOne("User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Event", b =>
                {
                    b.HasOne("User", "Organizer")
                        .WithMany("OrganizedEvents")
                        .HasForeignKey("OrganizerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Organizer");
                });

            modelBuilder.Entity("EventSettings", b =>
                {
                    b.HasOne("Event", "Event")
                        .WithOne("Settings")
                        .HasForeignKey("EventSettings", "EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("Media", b =>
                {
                    b.HasOne("Event", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("User", "Uploader")
                        .WithMany()
                        .HasForeignKey("UploaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("Payment", b =>
                {
                    b.HasOne("Coupon", "Coupon")
                        .WithMany("Payments")
                        .HasForeignKey("CouponId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("User", "User")
                        .WithMany("Payments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Coupon");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Content", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("Coupon", b =>
                {
                    b.Navigation("Payments");
                });

            modelBuilder.Entity("Event", b =>
                {
                    b.Navigation("Contents");

                    b.Navigation("Settings");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Navigation("CanvaDesigns");

                    b.Navigation("Comments");

                    b.Navigation("OrganizedEvents");

                    b.Navigation("Payments");

                    b.Navigation("UploadedContents");
                });
#pragma warning restore 612, 618
        }
    }
}
