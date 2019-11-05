using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class SeatNumberToBodySeatNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeatNumber",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                newName: "BodySeatNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BodySeatNumber",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                newName: "SeatNumber");
        }
    }
}
