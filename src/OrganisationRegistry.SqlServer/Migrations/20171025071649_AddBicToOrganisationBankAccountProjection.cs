using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBicToOrganisationBankAccountProjection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bic",
                schema: "OrganisationRegistry",
                table: "OrganisationBankAccountList",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBic",
                schema: "OrganisationRegistry",
                table: "OrganisationBankAccountList",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bic",
                schema: "OrganisationRegistry",
                table: "OrganisationBankAccountList");

            migrationBuilder.DropColumn(
                name: "IsBic",
                schema: "OrganisationRegistry",
                table: "OrganisationBankAccountList");
        }
    }
}
