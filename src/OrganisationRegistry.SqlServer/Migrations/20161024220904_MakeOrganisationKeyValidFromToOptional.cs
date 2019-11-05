using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class MakeOrganisationKeyValidFromToOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                nullable: false);
        }
    }
}
