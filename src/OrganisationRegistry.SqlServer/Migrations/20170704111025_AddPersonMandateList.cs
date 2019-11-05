using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddPersonMandateList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BodyOrganisationId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyOrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodySeatNumber",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FunctionTypeId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FunctionTypeName",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyOrganisationId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.DropColumn(
                name: "BodyOrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.DropColumn(
                name: "BodySeatNumber",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.DropColumn(
                name: "FunctionTypeId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.DropColumn(
                name: "FunctionTypeName",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.DropColumn(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");
        }
    }
}
