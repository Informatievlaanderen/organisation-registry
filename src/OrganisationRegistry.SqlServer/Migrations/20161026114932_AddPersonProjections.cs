using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddPersonProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "PersonList",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                schema: "OrganisationRegistry",
                table: "PersonList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "OrganisationRegistry",
                table: "PersonList",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                schema: "OrganisationRegistry",
                table: "PersonList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "PersonList",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                schema: "OrganisationRegistry",
                table: "PersonList",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "OrganisationRegistry",
                table: "PersonList",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                schema: "OrganisationRegistry",
                table: "PersonList",
                nullable: false);
        }
    }
}
