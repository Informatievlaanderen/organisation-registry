using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class ChangeGenderToSexInPeople : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gender",
                schema: "OrganisationRegistry",
                table: "PersonList",
                newName:"Sex");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sex",
                schema: "OrganisationRegistry",
                table: "PersonList",
                newName: "Gender");
        }
    }
}
