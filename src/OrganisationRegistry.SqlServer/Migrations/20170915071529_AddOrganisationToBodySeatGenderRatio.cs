using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationToBodySeatGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganisationShortName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");

            migrationBuilder.DropColumn(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");

            migrationBuilder.DropColumn(
                name: "OrganisationShortName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");
        }
    }
}
