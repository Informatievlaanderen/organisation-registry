using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodySeatListView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodySeatList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodySeatId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    SeatName = table.Column<string>(maxLength: 500, nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatList", x => x.BodySeatId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodySeatList_SeatName",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                column: "SeatName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_BodySeatList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_BodySeatList_ValidTo",
                schema: "OrganisationRegistry",
                table: "BodySeatList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodySeatList",
                schema: "OrganisationRegistry");
        }
    }
}
