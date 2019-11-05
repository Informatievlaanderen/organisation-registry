using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddLocationToOrganisationCapacityList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                maxLength: 460,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationCapacityList_LocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                column: "LocationName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganisationCapacityList_LocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList");

            migrationBuilder.DropColumn(
                name: "LocationId",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList");

            migrationBuilder.DropColumn(
                name: "LocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList");
        }
    }
}
