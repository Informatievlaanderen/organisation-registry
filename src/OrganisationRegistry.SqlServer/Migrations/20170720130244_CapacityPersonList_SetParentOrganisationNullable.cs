using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CapacityPersonList_SetParentOrganisationNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ParentOrganisationId",
                schema: "OrganisationRegistry",
                table: "CapacityPersonList",
                nullable: true,
                oldClrType: typeof(Guid));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ParentOrganisationId",
                schema: "OrganisationRegistry",
                table: "CapacityPersonList",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
