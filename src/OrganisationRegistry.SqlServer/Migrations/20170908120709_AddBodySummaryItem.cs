using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodySummaryItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");

            migrationBuilder.DropColumn(
                name: "BodyNumber",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");

            migrationBuilder.DropColumn(
                name: "BodyShortName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");

            migrationBuilder.CreateTable(
                name: "BodySummary",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    NrOfBodySeats = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySummary", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodySummary",
                schema: "OrganisationRegistry");

            migrationBuilder.AddColumn<string>(
                name: "BodyName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyNumber",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyShortName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: true);
        }
    }
}
