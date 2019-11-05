using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddAssignedToDelegations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToId",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedToName",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                maxLength: 401,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedToId",
                schema: "OrganisationRegistry",
                table: "BodyMandateList");

            migrationBuilder.DropColumn(
                name: "AssignedToName",
                schema: "OrganisationRegistry",
                table: "BodyMandateList");
        }
    }
}
