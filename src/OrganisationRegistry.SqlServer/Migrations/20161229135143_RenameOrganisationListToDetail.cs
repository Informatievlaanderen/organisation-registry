using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RenameOrganisationListToDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "PK_OrganisationList",
                newName: "PK_OrganisationDetail",
                table: "OrganisationList",
                schema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationList",
                newName: "OrganisationDetail",
                schema: "OrganisationRegistry");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "OrganisationDetail",
                newName: "OrganisationList",
                schema: "OrganisationRegistry");

            migrationBuilder.RenameIndex(
                name: "PK_OrganisationDetail",
                newName: "PK_OrganisationList",
                table: "OrganisationList",
                schema: "OrganisationRegistry");
        }
    }
}
