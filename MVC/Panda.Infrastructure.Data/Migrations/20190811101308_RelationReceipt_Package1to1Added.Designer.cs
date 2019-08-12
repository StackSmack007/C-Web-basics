﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Panda.Infrastructure.Data;

namespace Panda.Infrastructure.Data.Migrations
{
    [DbContext(typeof(PandaContext))]
    [Migration("20190811101308_RelationReceipt_Package1to1Added")]
    partial class RelationReceipt_Package1to1Added
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Panda.Indfrastructure.Models.Models.Package", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasMaxLength(10240);

                    b.Property<DateTime?>("EstimatedDeliveryDate");

                    b.Property<int?>("ReceiptId");

                    b.Property<string>("ShippingAddress")
                        .HasMaxLength(512);

                    b.Property<int>("Status");

                    b.Property<int>("UserId");

                    b.Property<decimal>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("Panda.Indfrastructure.Models.Models.Receipt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Fee");

                    b.Property<DateTime>("IssuedOn");

                    b.Property<int>("PackageId");

                    b.Property<int>("RecipientId");

                    b.Property<string>("ShippingAddress")
                        .HasMaxLength(512);

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("PackageId")
                        .IsUnique();

                    b.HasIndex("RecipientId");

                    b.ToTable("Receipts");
                });

            modelBuilder.Entity("Panda.Indfrastructure.Models.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<int>("Role");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Panda.Indfrastructure.Models.Models.Package", b =>
                {
                    b.HasOne("Panda.Indfrastructure.Models.Models.User", "User")
                        .WithMany("Packages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Panda.Indfrastructure.Models.Models.Receipt", b =>
                {
                    b.HasOne("Panda.Indfrastructure.Models.Models.Package", "Package")
                        .WithOne("Receipt")
                        .HasForeignKey("Panda.Indfrastructure.Models.Models.Receipt", "PackageId");

                    b.HasOne("Panda.Indfrastructure.Models.Models.User", "Recipient")
                        .WithMany("Receipts")
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
