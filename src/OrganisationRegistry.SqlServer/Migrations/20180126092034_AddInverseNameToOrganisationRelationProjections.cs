using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddInverseNameToOrganisationRelationProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RelationInverseName",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationList",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelationInverseName",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationList");
        }
    }
}
