using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyListViewValidityDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FormalValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyList",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FormalValidTo",
                schema: "OrganisationRegistry",
                table: "BodyList",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyList_FormalValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyList",
                column: "FormalValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_BodyList_FormalValidTo",
                schema: "OrganisationRegistry",
                table: "BodyList",
                column: "FormalValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BodyList_FormalValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyList");

            migrationBuilder.DropIndex(
                name: "IX_BodyList_FormalValidTo",
                schema: "OrganisationRegistry",
                table: "BodyList");

            migrationBuilder.DropColumn(
                name: "FormalValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyList");

            migrationBuilder.DropColumn(
                name: "FormalValidTo",
                schema: "OrganisationRegistry",
                table: "BodyList");
        }
    }
}
