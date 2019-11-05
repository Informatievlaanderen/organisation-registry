using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddActiveFlagsToBodySeatGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BodyActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OrganisationActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList");

            migrationBuilder.DropColumn(
                name: "OrganisationActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList");
        }
    }
}
