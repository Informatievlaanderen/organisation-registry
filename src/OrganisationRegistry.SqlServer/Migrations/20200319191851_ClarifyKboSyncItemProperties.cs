using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class ClarifyKboSyncItemProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceAddressModifiedAt",
                schema: "Magda",
                table: "KboSyncQueue");

            migrationBuilder.RenameColumn(
                name: "SourceName",
                schema: "Magda",
                table: "KboSyncQueue",
                newName: "SourceOrganisationName");

            migrationBuilder.RenameColumn(
                name: "SourceModifiedAt",
                schema: "Magda",
                table: "KboSyncQueue",
                newName: "SourceOrganisationModifiedAt");

            migrationBuilder.RenameColumn(
                name: "SourceKboNumber",
                schema: "Magda",
                table: "KboSyncQueue",
                newName: "SourceOrganisationKboNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SourceOrganisationName",
                schema: "Magda",
                table: "KboSyncQueue",
                newName: "SourceName");

            migrationBuilder.RenameColumn(
                name: "SourceOrganisationModifiedAt",
                schema: "Magda",
                table: "KboSyncQueue",
                newName: "SourceModifiedAt");

            migrationBuilder.RenameColumn(
                name: "SourceOrganisationKboNumber",
                schema: "Magda",
                table: "KboSyncQueue",
                newName: "SourceKboNumber");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SourceAddressModifiedAt",
                schema: "Magda",
                table: "KboSyncQueue",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
