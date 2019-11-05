using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyOrganisationIdToOrganisationPerBody : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BodyOrganisationId",
                schema: "OrganisationRegistry",
                table: "OrganisationPerBodyList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyOrganisationId",
                schema: "OrganisationRegistry",
                table: "OrganisationPerBodyList");
        }
    }
}
