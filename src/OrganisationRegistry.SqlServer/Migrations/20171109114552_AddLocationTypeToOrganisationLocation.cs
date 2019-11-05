using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddLocationTypeToOrganisationLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LocationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList");

            migrationBuilder.DropColumn(
                name: "LocationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList");
        }
    }
}
