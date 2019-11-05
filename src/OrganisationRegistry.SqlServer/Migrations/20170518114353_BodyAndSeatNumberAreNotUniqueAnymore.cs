using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class BodyAndSeatNumberAreNotUniqueAnymore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_BodySeatList_SeatNumber",
            //    schema: "OrganisationRegistry",
            //    table: "BodySeatList");

            //migrationBuilder.DropIndex(
            //    name: "IX_BodyDetail_BodyNumber",
            //    schema: "OrganisationRegistry",
            //    table: "BodyDetail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateIndex(
            //    name: "IX_BodySeatList_SeatNumber",
            //    schema: "OrganisationRegistry",
            //    table: "BodySeatList",
            //    column: "SeatNumber",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_BodyDetail_BodyNumber",
            //    schema: "OrganisationRegistry",
            //    table: "BodyDetail",
            //    column: "BodyNumber",
            //    unique: true);
        }
    }
}
