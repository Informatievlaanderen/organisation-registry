using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddPurposeNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Purposes",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                newName: "PurposeIds");

            migrationBuilder.AddColumn<string>(
                name: "PurposeNames",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PurposeIds",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                newName: "Purposes");

            migrationBuilder.DropColumn(
                name: "PurposeNames",
                schema: "OrganisationRegistry",
                table: "OrganisationList");
        }
    }
}
