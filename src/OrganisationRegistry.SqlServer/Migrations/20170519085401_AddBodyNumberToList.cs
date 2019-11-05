using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyNumberToList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodyNumber",
                schema: "OrganisationRegistry",
                table: "BodyList",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BodyList_BodyNumber",
                schema: "OrganisationRegistry",
                table: "BodyList",
                column: "BodyNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BodyList_BodyNumber",
                schema: "OrganisationRegistry",
                table: "BodyList");

            migrationBuilder.DropColumn(
                name: "BodyNumber",
                schema: "OrganisationRegistry",
                table: "BodyList");
        }
    }
}
