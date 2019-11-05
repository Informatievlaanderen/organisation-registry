using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RemoveShortNamesFromBodySeatGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyShortName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");

            migrationBuilder.DropColumn(
                name: "OrganisationShortName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodyShortName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganisationShortName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: true);
        }
    }
}
