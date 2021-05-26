using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RenameAndMakeNullableLastUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                schema: "OrganisationRegistry",
                table: "ProjectionStateList");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedUtc",
                schema: "OrganisationRegistry",
                table: "ProjectionStateList",
                type: "datetimeoffset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                schema: "OrganisationRegistry",
                table: "ProjectionStateList");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdated",
                schema: "OrganisationRegistry",
                table: "ProjectionStateList",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
