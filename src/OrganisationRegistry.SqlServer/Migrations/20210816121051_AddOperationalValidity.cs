using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOperationalValidity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OperationalValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OperationalValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationalValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail");

            migrationBuilder.DropColumn(
                name: "OperationalValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail");
        }
    }
}
