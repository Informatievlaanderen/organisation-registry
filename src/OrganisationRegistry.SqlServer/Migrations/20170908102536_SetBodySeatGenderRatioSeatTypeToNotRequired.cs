using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class SetBodySeatGenderRatioSeatTypeToNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BodySeatTypeName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BodySeatTypeName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
