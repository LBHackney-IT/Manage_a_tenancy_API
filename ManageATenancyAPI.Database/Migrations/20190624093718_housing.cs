using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManageATenancyAPI.Database.Migrations
{
    public partial class housing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HousingAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LastModified = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousingAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HousingAreaPatches",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LastModified = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    CrmId = table.Column<string>(nullable: true),
                    HousingAreaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousingAreaPatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HousingAreaPatches_HousingAreas_HousingAreaId",
                        column: x => x.HousingAreaId,
                        principalTable: "HousingAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TRAs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LastModified = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    HousingAreaPatchId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRAs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TRAs_HousingAreaPatches_HousingAreaPatchId",
                        column: x => x.HousingAreaPatchId,
                        principalTable: "HousingAreaPatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TRAEstate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LastModified = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    UHReference = table.Column<string>(nullable: true),
                    TRAId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRAEstate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TRAEstate_TRAs_TRAId",
                        column: x => x.TRAId,
                        principalTable: "TRAs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HousingAreaPatches_HousingAreaId",
                table: "HousingAreaPatches",
                column: "HousingAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_TRAEstate_TRAId",
                table: "TRAEstate",
                column: "TRAId");

            migrationBuilder.CreateIndex(
                name: "IX_TRAs_HousingAreaPatchId",
                table: "TRAs",
                column: "HousingAreaPatchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TRAEstate");

            migrationBuilder.DropTable(
                name: "TRAs");

            migrationBuilder.DropTable(
                name: "HousingAreaPatches");

            migrationBuilder.DropTable(
                name: "HousingAreas");
        }
    }
}
