using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class DropRequirementForBodyAndBodySeatNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BodySeatNumber",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "BodyNumber",
                schema: "OrganisationRegistry",
                table: "BodyList",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "BodyNumber",
                schema: "OrganisationRegistry",
                table: "BodyDetail",
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
                table: "BodySeatList",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BodyNumber",
                schema: "OrganisationRegistry",
                table: "BodyList",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BodyNumber",
                schema: "OrganisationRegistry",
                table: "BodyDetail",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10,
                oldNullable: true);
        }
    }
}
