using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddRelatedOrganisationToOrganisationRelationList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RelatedOrganisationId",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "RelatedOrganisationName",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationList",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedOrganisationId",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationList");

            migrationBuilder.DropColumn(
                name: "RelatedOrganisationName",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationList");
        }
    }
}
