using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddSearchFieldsToOrganisationProjection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationClassificationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OrganisationClassificationName",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OrganisationClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "OrganisationClassificationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "OrganisationClassificationName",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "OrganisationClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationList");
        }
    }
}
