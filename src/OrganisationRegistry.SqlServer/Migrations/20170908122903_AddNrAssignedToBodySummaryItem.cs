using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddNrAssignedToBodySummaryItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NrOfAssignedFunctionsToBodySeats",
                schema: "OrganisationRegistry",
                table: "BodySummary",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NrOfAssignedOrganisationsToBodySeats",
                schema: "OrganisationRegistry",
                table: "BodySummary",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NrOfAssignedPersonsToBodySeats",
                schema: "OrganisationRegistry",
                table: "BodySummary",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NrOfAssignedFunctionsToBodySeats",
                schema: "OrganisationRegistry",
                table: "BodySummary");

            migrationBuilder.DropColumn(
                name: "NrOfAssignedOrganisationsToBodySeats",
                schema: "OrganisationRegistry",
                table: "BodySummary");

            migrationBuilder.DropColumn(
                name: "NrOfAssignedPersonsToBodySeats",
                schema: "OrganisationRegistry",
                table: "BodySummary");
        }
    }
}
