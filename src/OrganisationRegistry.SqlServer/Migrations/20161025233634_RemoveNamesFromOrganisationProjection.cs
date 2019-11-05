using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RemoveNamesFromOrganisationProjection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "OrganisationClassificationName",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "OrganisationClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationList");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganisationClassificationName",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganisationClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                maxLength: 500,
                nullable: true);
        }
    }
}
