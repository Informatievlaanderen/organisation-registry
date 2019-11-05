using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CascadeOrganisationListDeletes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganisationFormalFrameworkValidity_OrganisationList_OrganisationListItemId",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganisationFormalFrameworkValidity_OrganisationList_OrganisationListItemId",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity",
                column: "OrganisationListItemId",
                principalSchema: "OrganisationRegistry",
                principalTable: "OrganisationList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganisationFormalFrameworkValidity_OrganisationList_OrganisationListItemId",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganisationFormalFrameworkValidity_OrganisationList_OrganisationListItemId",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity",
                column: "OrganisationListItemId",
                principalSchema: "OrganisationRegistry",
                principalTable: "OrganisationList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
