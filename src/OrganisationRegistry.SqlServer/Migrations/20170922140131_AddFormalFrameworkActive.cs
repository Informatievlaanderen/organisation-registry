using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddFormalFrameworkActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FormalFrameworkActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FormalFrameworkActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_FormalFrameworkPerBodyList",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormalFrameworkActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList");

            migrationBuilder.DropColumn(
                name: "FormalFrameworkActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_FormalFrameworkPerBodyList");
        }
    }
}
