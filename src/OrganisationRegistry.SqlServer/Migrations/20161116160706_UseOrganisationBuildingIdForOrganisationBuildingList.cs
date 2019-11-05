using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class UseOrganisationBuildingIdForOrganisationBuildingList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationBuildingList",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationBuildingId",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationBuildingList",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList",
                column: "OrganisationBuildingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationBuildingList",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList");

            migrationBuilder.DropColumn(
                name: "OrganisationBuildingId",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationBuildingList",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList",
                columns: new[] { "OrganisationId", "BuildingId" });
        }
    }
}
