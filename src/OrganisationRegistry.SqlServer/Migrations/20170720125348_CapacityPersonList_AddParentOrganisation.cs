using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CapacityPersonList_AddParentOrganisation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentOrganisationId",
                schema: "OrganisationRegistry",
                table: "CapacityPersonList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ParentOrganisationName",
                schema: "OrganisationRegistry",
                table: "CapacityPersonList",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentOrganisationId",
                schema: "OrganisationRegistry",
                table: "CapacityPersonList");

            migrationBuilder.DropColumn(
                name: "ParentOrganisationName",
                schema: "OrganisationRegistry",
                table: "CapacityPersonList");
        }
    }
}
