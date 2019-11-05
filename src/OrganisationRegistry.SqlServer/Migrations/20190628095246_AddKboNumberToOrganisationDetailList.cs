using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddKboNumberToOrganisationDetailList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KboNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KboNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationDetail");
        }
    }
}
