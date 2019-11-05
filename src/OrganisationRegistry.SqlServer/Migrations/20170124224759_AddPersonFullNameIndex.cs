using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddPersonFullNameIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PersonList_FullName",
                schema: "OrganisationRegistry",
                table: "PersonList",
                column: "FullName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PersonList_FullName",
                schema: "OrganisationRegistry",
                table: "PersonList");
        }
    }
}
