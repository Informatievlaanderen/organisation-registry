using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationBuildingListProjection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationBuildingList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationId = table.Column<Guid>(nullable: false),
                    BuildingId = table.Column<Guid>(nullable: false),
                    BuildingName = table.Column<string>(maxLength: 500, nullable: true),
                    IsMainBuilding = table.Column<bool>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationBuildingList", x => new { x.OrganisationId, x.BuildingId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationBuildingList",
                schema: "OrganisationRegistry");
        }
    }
}
