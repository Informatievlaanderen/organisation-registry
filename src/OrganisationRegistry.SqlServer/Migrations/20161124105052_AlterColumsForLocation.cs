using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AlterColumsForLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.DropColumn(
                name: "Street",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine",
                schema: "OrganisationRegistry",
                table: "LocationList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                schema: "OrganisationRegistry",
                table: "LocationList",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "OrganisationRegistry",
                table: "LocationList",
                maxLength: 100,
                nullable: true);
        }
    }
}
