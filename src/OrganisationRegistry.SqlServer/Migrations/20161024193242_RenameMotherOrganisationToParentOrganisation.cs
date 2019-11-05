using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RenameMotherOrganisationToParentOrganisation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MotherOrganisation",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "MotherOrganisationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.AddColumn<string>(
                name: "ParentOrganisation",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentOrganisationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentOrganisation",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "ParentOrganisationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.AddColumn<string>(
                name: "MotherOrganisation",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MotherOrganisationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
