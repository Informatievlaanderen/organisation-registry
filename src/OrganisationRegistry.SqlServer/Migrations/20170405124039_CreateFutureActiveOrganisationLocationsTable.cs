using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateFutureActiveOrganisationLocationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FutureActiveOrganisationLocationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationLocationId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureActiveOrganisationLocationList", x => x.OrganisationLocationId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FutureActiveOrganisationLocationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "FutureActiveOrganisationLocationList",
                column: "ValidFrom");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FutureActiveOrganisationLocationList",
                schema: "OrganisationRegistry");
        }
    }
}
