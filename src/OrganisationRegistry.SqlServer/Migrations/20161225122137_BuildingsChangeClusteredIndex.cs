using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class BuildingsChangeClusteredIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BuildingList",
                schema: "OrganisationRegistry",
                table: "BuildingList");

            migrationBuilder.DropIndex(
                name: "IX_BuildingList_Name",
                schema: "OrganisationRegistry",
                table: "BuildingList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BuildingList",
                schema: "OrganisationRegistry",
                table: "BuildingList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_BuildingList_Name",
                schema: "OrganisationRegistry",
                table: "BuildingList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BuildingList",
                schema: "OrganisationRegistry",
                table: "BuildingList");

            migrationBuilder.DropIndex(
                name: "IX_BuildingList_Name",
                schema: "OrganisationRegistry",
                table: "BuildingList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BuildingList",
                schema: "OrganisationRegistry",
                table: "BuildingList",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingList_Name",
                schema: "OrganisationRegistry",
                table: "BuildingList",
                column: "Name",
                unique: true);
        }
    }
}
