using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddHasCrabLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasCrabLocation",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_LocationList_HasCrabLocation",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "HasCrabLocation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LocationList_HasCrabLocation",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.DropColumn(
                name: "HasCrabLocation",
                schema: "OrganisationRegistry",
                table: "LocationList");
        }
    }
}
