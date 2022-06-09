using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AlterTable_OrganisationDetailList_AddSourceColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SourceId",
                schema: "Backoffice",
                table: "OrganisationDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceOrganisationIdentifier",
                schema: "Backoffice",
                table: "OrganisationDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                schema: "Backoffice",
                table: "OrganisationDetail",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceId",
                schema: "Backoffice",
                table: "OrganisationDetail");

            migrationBuilder.DropColumn(
                name: "SourceOrganisationIdentifier",
                schema: "Backoffice",
                table: "OrganisationDetail");

            migrationBuilder.DropColumn(
                name: "SourceType",
                schema: "Backoffice",
                table: "OrganisationDetail");
        }
    }
}
