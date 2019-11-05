using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class UpdateOrganisationKeysWithItsOwnGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationKeyList",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationKeyId",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationKeyList",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                column: "OrganisationKeyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationKeyList",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList");

            migrationBuilder.DropColumn(
                name: "OrganisationKeyId",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationKeyList",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                columns: new[] { "OrganisationId", "KeyId" });
        }
    }
}
