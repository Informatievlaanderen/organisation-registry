using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationToBodies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ParentOrganisation",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Organisation",
                schema: "OrganisationRegistry",
                table: "BodyList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "BodyList",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyList_Organisation",
                schema: "OrganisationRegistry",
                table: "BodyList",
                column: "Organisation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BodyList_Organisation",
                schema: "OrganisationRegistry",
                table: "BodyList");

            migrationBuilder.DropColumn(
                name: "Organisation",
                schema: "OrganisationRegistry",
                table: "BodyList");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "BodyList");

            migrationBuilder.AlterColumn<string>(
                name: "ParentOrganisation",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
