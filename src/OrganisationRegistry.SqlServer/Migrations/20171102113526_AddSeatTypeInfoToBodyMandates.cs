using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddSeatTypeInfoToBodyMandates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BodySeatTypeId",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodySeatTypeName",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BodySeatTypeOrder",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodySeatTypeId",
                schema: "OrganisationRegistry",
                table: "BodyMandateList");

            migrationBuilder.DropColumn(
                name: "BodySeatTypeName",
                schema: "OrganisationRegistry",
                table: "BodyMandateList");

            migrationBuilder.DropColumn(
                name: "BodySeatTypeOrder",
                schema: "OrganisationRegistry",
                table: "BodyMandateList");
        }
    }
}
