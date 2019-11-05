using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddFormalFrameworkToBodySeatGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList");

            migrationBuilder.DropColumn(
                name: "FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList");
        }
    }
}
