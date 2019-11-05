using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddDelegatorsToBodyMandates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Representation",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                newName: "DelegatorName");

            migrationBuilder.RenameIndex(
                name: "IX_BodyMandateList_Representation",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                newName: "IX_BodyMandateList_DelegatorName");

            migrationBuilder.AddColumn<int>(
                name: "BodyMandateType",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "DelegatedId",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DelegatedName",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DelegatorId",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyMandateType",
                schema: "OrganisationRegistry",
                table: "BodyMandateList");

            migrationBuilder.DropColumn(
                name: "DelegatedId",
                schema: "OrganisationRegistry",
                table: "BodyMandateList");

            migrationBuilder.DropColumn(
                name: "DelegatedName",
                schema: "OrganisationRegistry",
                table: "BodyMandateList");

            migrationBuilder.DropColumn(
                name: "DelegatorId",
                schema: "OrganisationRegistry",
                table: "BodyMandateList");

            migrationBuilder.RenameColumn(
                name: "DelegatorName",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                newName: "Representation");

            migrationBuilder.RenameIndex(
                name: "IX_BodyMandateList_DelegatorName",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                newName: "IX_BodyMandateList_Representation");
        }
    }
}
