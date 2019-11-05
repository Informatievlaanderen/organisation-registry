using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddSeatNumberToBodySeatList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatNumber",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

//            // This will be added again later
//            migrationBuilder.CreateIndex(
//                name: "IX_BodySeatList_SeatNumber",
//                schema: "OrganisationRegistry",
//                table: "BodySeatList",
//                column: "SeatNumber",
//                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
//            // This will be added again later
//            migrationBuilder.DropIndex(
//                name: "IX_BodySeatList_SeatNumber",
//                schema: "OrganisationRegistry",
//                table: "BodySeatList");

            migrationBuilder.DropColumn(
                name: "SeatNumber",
                schema: "OrganisationRegistry",
                table: "BodySeatList");
        }
    }
}
