using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddParentOvoNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentOrganisationOvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentOrganisationOvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationList");
        }
    }
}
