using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyLifecycleAdjecentGaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAdjacentGaps",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseList",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAdjacentGaps",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseList");
        }
    }
}
