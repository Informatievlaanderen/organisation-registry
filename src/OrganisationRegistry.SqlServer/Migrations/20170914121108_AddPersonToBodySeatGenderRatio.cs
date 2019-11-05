using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddPersonToBodySeatGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodyName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonFullName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                maxLength: 401,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PersonId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");

            migrationBuilder.DropColumn(
                name: "PersonFullName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");

            migrationBuilder.DropColumn(
                name: "PersonId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");
        }
    }
}
