using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateFutureActiveOrganisationBuildingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FutureActiveOrganisationBuildingList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationBuildingId = table.Column<Guid>(nullable: false),
                    BuildingId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureActiveOrganisationBuildingList", x => x.OrganisationBuildingId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FutureActiveOrganisationBuildingList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "FutureActiveOrganisationBuildingList",
                column: "ValidFrom");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FutureActiveOrganisationBuildingList",
                schema: "OrganisationRegistry");
        }
    }
}
