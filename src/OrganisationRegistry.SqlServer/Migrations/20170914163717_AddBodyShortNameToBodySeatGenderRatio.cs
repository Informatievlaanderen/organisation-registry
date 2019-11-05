using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyShortNameToBodySeatGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodyShortName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyShortName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");
        }
    }
}
