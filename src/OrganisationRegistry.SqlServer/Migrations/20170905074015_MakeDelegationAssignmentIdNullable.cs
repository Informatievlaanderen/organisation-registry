using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class MakeDelegationAssignmentIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonMandateList",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegationAssignmentId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "PersonMandateId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonMandateList",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                column: "PersonMandateId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_PersonMandateList_BodyMandateId_DelegationAssignmentId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                columns: new[] { "BodyMandateId", "DelegationAssignmentId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonMandateList",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.DropIndex(
                name: "IX_PersonMandateList_BodyMandateId_DelegationAssignmentId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.DropColumn(
                name: "PersonMandateId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegationAssignmentId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonMandateList",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                columns: new[] { "BodyMandateId", "DelegationAssignmentId" })
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
