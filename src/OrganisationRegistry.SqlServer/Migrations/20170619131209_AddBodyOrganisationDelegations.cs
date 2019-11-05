using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyOrganisationDelegations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BodyOrganisationId",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "BodyOrganisationName",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyOrganisationId",
                schema: "OrganisationRegistry",
                table: "DelegationList");

            migrationBuilder.DropColumn(
                name: "BodyOrganisationName",
                schema: "OrganisationRegistry",
                table: "DelegationList");
        }
    }
}
