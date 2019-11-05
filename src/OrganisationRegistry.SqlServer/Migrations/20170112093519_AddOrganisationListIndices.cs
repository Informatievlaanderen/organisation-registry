using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationListIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationFormalFrameworkValidity",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "FormalFrameworkId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_OrganisationClassificationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "OrganisationClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "OrganisationClassificationTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationFormalFrameworkValidity",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity",
                column: "Id")
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_OrganisationClassificationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationFormalFrameworkValidity",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationFormalFrameworkValidity",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
