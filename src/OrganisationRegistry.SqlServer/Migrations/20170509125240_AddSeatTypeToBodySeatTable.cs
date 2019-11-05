using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddSeatTypeToBodySeatTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BodySeatList_SeatName",
                schema: "OrganisationRegistry",
                table: "BodySeatList");

            migrationBuilder.RenameColumn(
                name: "SeatName",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                newName: "SeatTypeName");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "SeatTypeId",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BodySeatList_Name",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                column: "Name")
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BodySeatList_Name",
                schema: "OrganisationRegistry",
                table: "BodySeatList");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "BodySeatList");

            migrationBuilder.DropColumn(
                name: "SeatTypeId",
                schema: "OrganisationRegistry",
                table: "BodySeatList");

            migrationBuilder.RenameColumn(
                name: "SeatTypeName",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                newName: "SeatName");

            migrationBuilder.CreateIndex(
                name: "IX_BodySeatList_SeatName",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                column: "SeatName")
                .Annotation("SqlServer:Clustered", true);
        }
    }
}
