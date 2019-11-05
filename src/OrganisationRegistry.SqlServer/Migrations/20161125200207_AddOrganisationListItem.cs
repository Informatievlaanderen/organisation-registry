using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationListItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationLocationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationLocationId = table.Column<Guid>(nullable: false),
                    IsMainLocation = table.Column<bool>(nullable: false),
                    LocationId = table.Column<Guid>(nullable: false),
                    LocationName = table.Column<string>(maxLength: 500, nullable: true),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationLocationList", x => x.OrganisationLocationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationLocationList",
                schema: "OrganisationRegistry");

         }
    }
}
