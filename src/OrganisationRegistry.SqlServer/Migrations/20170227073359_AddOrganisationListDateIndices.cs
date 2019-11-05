using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationListDateIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFormalFrameworkValidity_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFormalFrameworkValidity_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationClassificationValidity_OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationValidity",
                column: "OrganisationClassificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationClassificationValidity_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationValidity",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationClassificationValidity_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationValidity",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationFormalFrameworkValidity_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationFormalFrameworkValidity_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationClassificationValidity_OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationValidity");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationClassificationValidity_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationValidity");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationClassificationValidity_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationValidity");
        }
    }
}
