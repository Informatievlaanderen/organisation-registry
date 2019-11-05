using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RemovePersonNameFromPersonFunctionList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PersonName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                maxLength: 500,
                nullable: true);
        }
    }
}
