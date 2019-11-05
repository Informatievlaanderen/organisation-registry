using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddShortNameIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                schema: "OrganisationRegistry",
                table: "BodyList",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_ShortName",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "ShortName");

            migrationBuilder.CreateIndex(
                name: "IX_BodyList_ShortName",
                schema: "OrganisationRegistry",
                table: "BodyList",
                column: "ShortName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_ShortName",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropIndex(
                name: "IX_BodyList_ShortName",
                schema: "OrganisationRegistry",
                table: "BodyList");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                schema: "OrganisationRegistry",
                table: "BodyList",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
