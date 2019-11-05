using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RemovePersonActiveFromBodySeatGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PersonActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList",
                nullable: false,
                defaultValue: false);
        }
    }
}
