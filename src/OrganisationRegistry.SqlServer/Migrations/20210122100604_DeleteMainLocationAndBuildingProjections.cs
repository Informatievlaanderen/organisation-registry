using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class DeleteMainLocationAndBuildingProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveOrganisationBuildingList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "ActiveOrganisationLocationList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "FutureActiveOrganisationBuildingList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "FutureActiveOrganisationLocationList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropColumn(
                name: "MainBuildingId",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail");

            migrationBuilder.DropColumn(
                name: "MainBuildingName",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail");

            migrationBuilder.DropColumn(
                name: "MainLocationId",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail");

            migrationBuilder.DropColumn(
                name: "MainLocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MainBuildingId",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainBuildingName",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MainLocationId",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainLocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActiveOrganisationBuildingList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveOrganisationBuildingList", x => x.OrganisationBuildingId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ActiveOrganisationLocationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveOrganisationLocationList", x => x.OrganisationLocationId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "FutureActiveOrganisationBuildingList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureActiveOrganisationBuildingList", x => x.OrganisationBuildingId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "FutureActiveOrganisationLocationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureActiveOrganisationLocationList", x => x.OrganisationLocationId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveOrganisationBuildingList_ValidTo",
                schema: "OrganisationRegistry",
                table: "ActiveOrganisationBuildingList",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveOrganisationLocationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "ActiveOrganisationLocationList",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_FutureActiveOrganisationBuildingList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "FutureActiveOrganisationBuildingList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_FutureActiveOrganisationLocationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "FutureActiveOrganisationLocationList",
                column: "ValidFrom");
        }
    }
}
