using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddParentOrganisationsRelationshipIdToOrganisationList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentOrganisationsRelationshipId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentOrganisationsRelationshipId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");
        }
    }
}
