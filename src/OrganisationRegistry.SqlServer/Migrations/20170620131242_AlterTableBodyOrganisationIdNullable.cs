using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AlterTableBodyOrganisationIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "BodyOrganisationId",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                nullable: true,
                oldClrType: typeof(Guid));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "BodyOrganisationId",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
