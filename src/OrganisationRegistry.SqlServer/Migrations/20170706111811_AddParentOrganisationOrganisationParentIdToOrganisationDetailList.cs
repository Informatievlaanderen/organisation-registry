using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddParentOrganisationOrganisationParentIdToOrganisationDetailList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentOrganisationOrganisationParentId",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentOrganisationOrganisationParentId",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail");
        }
    }
}
