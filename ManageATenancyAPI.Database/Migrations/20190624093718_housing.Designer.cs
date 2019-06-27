﻿// <auto-generated />
using System;
using ManageATenancyAPI.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ManageATenancyAPI.Database.Migrations
{
    [DbContext(typeof(TenancyContext))]
    [Migration("20190624093718_housing")]
    partial class housing
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ManageATenancyAPI.Database.Models.HousingArea", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.ToTable("HousingAreas");
                });

            modelBuilder.Entity("ManageATenancyAPI.Database.Models.HousingAreaPatch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CrmId");

                    b.Property<int?>("HousingAreaId");

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("HousingAreaId");

                    b.ToTable("HousingAreaPatches");
                });

            modelBuilder.Entity("ManageATenancyAPI.Database.Models.NewTenancyLastRun", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("LastRun");

                    b.HasKey("Id");

                    b.ToTable("NewTenancyLastRun");
                });

            modelBuilder.Entity("ManageATenancyAPI.Database.Models.TRA", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int?>("HousingAreaPatchId");

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("HousingAreaPatchId");

                    b.ToTable("TRAs");
                });

            modelBuilder.Entity("ManageATenancyAPI.Database.Models.TRAEstate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<int?>("TRAId");

                    b.Property<string>("UHReference");

                    b.HasKey("Id");

                    b.HasIndex("TRAId");

                    b.ToTable("TRAEstate");
                });

            modelBuilder.Entity("ManageATenancyAPI.Database.Models.HousingAreaPatch", b =>
                {
                    b.HasOne("ManageATenancyAPI.Database.Models.HousingArea")
                        .WithMany("Patches")
                        .HasForeignKey("HousingAreaId");
                });

            modelBuilder.Entity("ManageATenancyAPI.Database.Models.TRA", b =>
                {
                    b.HasOne("ManageATenancyAPI.Database.Models.HousingAreaPatch")
                        .WithMany("TRAs")
                        .HasForeignKey("HousingAreaPatchId");
                });

            modelBuilder.Entity("ManageATenancyAPI.Database.Models.TRAEstate", b =>
                {
                    b.HasOne("ManageATenancyAPI.Database.Models.TRA")
                        .WithMany("Estates")
                        .HasForeignKey("TRAId");
                });
#pragma warning restore 612, 618
        }
    }
}
