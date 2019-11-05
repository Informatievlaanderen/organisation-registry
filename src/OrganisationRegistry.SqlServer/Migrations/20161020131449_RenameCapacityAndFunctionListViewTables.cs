using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RenameCapacityAndFunctionListViewTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FunctionListItem",
                schema: "OrganisationRegistry",
                table: "FunctionListItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CapacityListItem",
                schema: "OrganisationRegistry",
                table: "CapacityListItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FunctionList",
                schema: "OrganisationRegistry",
                table: "FunctionListItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CapacityList",
                schema: "OrganisationRegistry",
                table: "CapacityListItem",
                column: "Id");

            migrationBuilder.RenameTable(
                name: "FunctionListItem",
                schema: "OrganisationRegistry",
                newName: "FunctionList");

            migrationBuilder.RenameTable(
                name: "CapacityListItem",
                schema: "OrganisationRegistry",
                newName: "CapacityList");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FunctionList",
                schema: "OrganisationRegistry",
                table: "FunctionList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CapacityList",
                schema: "OrganisationRegistry",
                table: "CapacityList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FunctionListItem",
                schema: "OrganisationRegistry",
                table: "FunctionList",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CapacityListItem",
                schema: "OrganisationRegistry",
                table: "CapacityList",
                column: "Id");

            migrationBuilder.RenameTable(
                name: "FunctionList",
                schema: "OrganisationRegistry",
                newName: "FunctionListItem");

            migrationBuilder.RenameTable(
                name: "CapacityList",
                schema: "OrganisationRegistry",
                newName: "CapacityListItem");
        }
    }
}
