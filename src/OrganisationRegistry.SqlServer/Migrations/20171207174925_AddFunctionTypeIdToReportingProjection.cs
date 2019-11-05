using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddFunctionTypeIdToReportingProjection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FunctionTypeId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioBodyMandateList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FunctionTypeId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioBodyMandateList");
        }
    }
}
