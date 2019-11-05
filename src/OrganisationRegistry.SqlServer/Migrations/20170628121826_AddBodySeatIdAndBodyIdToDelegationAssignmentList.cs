using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodySeatIdAndBodyIdToDelegationAssignmentList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BodyId",
                schema: "OrganisationRegistry",
                table: "DelegationAssignmentList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "BodySeatId",
                schema: "OrganisationRegistry",
                table: "DelegationAssignmentList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyId",
                schema: "OrganisationRegistry",
                table: "DelegationAssignmentList");

            migrationBuilder.DropColumn(
                name: "BodySeatId",
                schema: "OrganisationRegistry",
                table: "DelegationAssignmentList");
        }
    }
}
