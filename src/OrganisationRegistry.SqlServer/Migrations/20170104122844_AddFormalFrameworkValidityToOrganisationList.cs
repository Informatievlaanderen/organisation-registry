using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddFormalFrameworkValidityToOrganisationList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FormalFrameworkValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FormalFrameworkValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormalFrameworkValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "FormalFrameworkValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationList");
        }
    }
}
