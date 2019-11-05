using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class DropBodySummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodySummary",
                schema: "OrganisationRegistry");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodySummary",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    NrOfAssignedFunctionsToBodySeats = table.Column<int>(nullable: false),
                    NrOfAssignedOrganisationsToBodySeats = table.Column<int>(nullable: false),
                    NrOfAssignedPersonsToBodySeats = table.Column<int>(nullable: false),
                    NrOfBodySeats = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySummary", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });
        }
    }
}
