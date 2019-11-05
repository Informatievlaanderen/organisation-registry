using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrderAndActiveToOrganisationClassification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                maxLength: 50,
                nullable: true);
        }
    }
}
