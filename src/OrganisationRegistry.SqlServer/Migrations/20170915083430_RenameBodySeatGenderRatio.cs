using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RenameBodySeatGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BodySeatGenderRatio",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatio",
                schema: "OrganisationRegistry",
                newName: "BodySeatGenderRatioList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BodySeatGenderRatioList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BodySeatGenderRatioList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioList",
                schema: "OrganisationRegistry",
                newName: "BodySeatGenderRatio");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BodySeatGenderRatio",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
