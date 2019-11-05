using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class SetPrimaryKeyToBodyMandateIdAndDelegationAssignmentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonMandateList",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.AddColumn<Guid>(
                name: "DelegationAssignmentId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonMandateList",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                columns: new[] { "BodyMandateId", "DelegationAssignmentId" })
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonMandateList",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.DropColumn(
                name: "DelegationAssignmentId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonMandateList",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                column: "BodyMandateId")
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
