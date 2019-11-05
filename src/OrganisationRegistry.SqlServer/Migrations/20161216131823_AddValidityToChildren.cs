using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddValidityToChildren : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationChildList",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.RenameColumn("Id", "OrganisationChildList", "OrganisationOrganisationParentId", "OrganisationRegistry");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("UPDATE OrganisationRegistry.OrganisationChildList SET Id = OrganisationOrganisationParentId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationChildList",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                column: "OrganisationOrganisationParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationChildList",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.DropColumn(
                name: "OrganisationOrganisationParentId",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationChildList",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                column: "Id");
        }
    }
}
