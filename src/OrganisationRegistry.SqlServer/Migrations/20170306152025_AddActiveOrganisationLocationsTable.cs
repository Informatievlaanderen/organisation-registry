using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddActiveOrganisationLocationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveOrganisationLocationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationLocationId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveOrganisationLocationList", x => x.OrganisationLocationId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveOrganisationLocationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "ActiveOrganisationLocationList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveOrganisationLocationList",
                schema: "OrganisationRegistry");
        }
    }
}
