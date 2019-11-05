using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class MakeBodyNumberOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BodySeatNumber",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BodySeatNumber",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10,
                oldNullable: true);
        }
    }
}
