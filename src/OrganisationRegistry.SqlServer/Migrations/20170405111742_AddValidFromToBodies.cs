using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddValidFromToBodies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyList",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "OrganisationRegistry",
                table: "BodyList",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_BodyList_ValidTo",
                schema: "OrganisationRegistry",
                table: "BodyList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BodyList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyList");

            migrationBuilder.DropIndex(
                name: "IX_BodyList_ValidTo",
                schema: "OrganisationRegistry",
                table: "BodyList");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyList");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "OrganisationRegistry",
                table: "BodyList");
        }
    }
}
