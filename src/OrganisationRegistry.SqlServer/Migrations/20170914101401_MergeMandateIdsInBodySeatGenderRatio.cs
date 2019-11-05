using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class MergeMandateIdsInBodySeatGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FunctionMandateId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");

            migrationBuilder.DropColumn(
                name: "OrganisationMandateId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");

            migrationBuilder.RenameColumn(
                name: "PersonMandateId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                newName: "BodyMandateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BodyMandateId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                newName: "PersonMandateId");

            migrationBuilder.AddColumn<Guid>(
                name: "FunctionMandateId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationMandateId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
