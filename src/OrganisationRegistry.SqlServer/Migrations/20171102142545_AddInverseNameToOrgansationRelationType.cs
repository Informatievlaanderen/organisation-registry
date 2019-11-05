using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddInverseNameToOrgansationRelationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InverseName",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationTypeList",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InverseName",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationTypeList");
        }
    }
}
