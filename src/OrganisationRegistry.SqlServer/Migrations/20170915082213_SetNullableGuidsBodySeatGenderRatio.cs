using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class SetNullableGuidsBodySeatGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "PersonId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "BodyMandateId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: true,
                oldClrType: typeof(Guid));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "PersonId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BodyMandateId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
