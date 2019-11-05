using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddKeyNameToOrganisationKeyListProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeyName",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyName",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList");
        }
    }
}
