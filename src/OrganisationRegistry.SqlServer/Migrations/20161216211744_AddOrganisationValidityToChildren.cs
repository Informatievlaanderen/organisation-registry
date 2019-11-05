using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationValidityToChildren : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OrganisationValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrganisationValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganisationValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.DropColumn(
                name: "OrganisationValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");
        }
    }
}
