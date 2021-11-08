using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddRegulationSubThemesToOrganisationRegulations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RegulationSubThemeId",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegulationSubThemeName",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegulationSubThemeId",
                schema: "Backoffice",
                table: "OrganisationRegulationList");

            migrationBuilder.DropColumn(
                name: "RegulationSubThemeName",
                schema: "Backoffice",
                table: "OrganisationRegulationList");
        }
    }
}
