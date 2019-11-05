using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodySeatIdToBodyMandatesScheduledActions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BodySeatId",
                schema: "OrganisationRegistry",
                table: "FuturePeopleAssignedToBodyMandatesList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "BodySeatId",
                schema: "OrganisationRegistry",
                table: "ActivePeopleAssignedToBodyMandatesList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodySeatId",
                schema: "OrganisationRegistry",
                table: "FuturePeopleAssignedToBodyMandatesList");

            migrationBuilder.DropColumn(
                name: "BodySeatId",
                schema: "OrganisationRegistry",
                table: "ActivePeopleAssignedToBodyMandatesList");
        }
    }
}
