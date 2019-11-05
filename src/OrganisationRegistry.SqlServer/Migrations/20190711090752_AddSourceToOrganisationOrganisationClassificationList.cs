using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddSourceToOrganisationOrganisationClassificationList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList");
        }
    }
}
