using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class MakeVimIdNumeric : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VimId",
                schema: "OrganisationRegistry",
                table: "BuildingList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "BuildingList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_BuildingList_Name",
                schema: "OrganisationRegistry",
                table: "BuildingList",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuildingList_VimId",
                schema: "OrganisationRegistry",
                table: "BuildingList",
                column: "VimId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BuildingList_Name",
                schema: "OrganisationRegistry",
                table: "BuildingList");

            migrationBuilder.DropIndex(
                name: "IX_BuildingList_VimId",
                schema: "OrganisationRegistry",
                table: "BuildingList");

            migrationBuilder.AlterColumn<string>(
                name: "VimId",
                schema: "OrganisationRegistry",
                table: "BuildingList",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "BuildingList",
                maxLength: 500,
                nullable: true);
        }
    }
}
