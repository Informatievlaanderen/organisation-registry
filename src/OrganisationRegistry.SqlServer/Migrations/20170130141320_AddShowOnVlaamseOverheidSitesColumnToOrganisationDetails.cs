using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddShowOnVlaamseOverheidSitesColumnToOrganisationDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowOnVlaamseOverheidSites",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowOnVlaamseOverheidSites",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail");
        }
    }
}
