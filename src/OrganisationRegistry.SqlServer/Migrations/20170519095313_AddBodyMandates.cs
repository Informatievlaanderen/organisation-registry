using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyMandates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyMandateList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyMandateId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    BodySeatId = table.Column<Guid>(nullable: false),
                    BodySeatName = table.Column<string>(maxLength: 500, nullable: false),
                    BodySeatNumber = table.Column<string>(maxLength: 10, nullable: false),
                    Representation = table.Column<string>(maxLength: 500, nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyMandateList", x => x.BodyMandateId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodyMandateList_BodySeatName",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                column: "BodySeatName");

            migrationBuilder.CreateIndex(
                name: "IX_BodyMandateList_Representation",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                column: "Representation")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyMandateList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_BodyMandateList_ValidTo",
                schema: "OrganisationRegistry",
                table: "BodyMandateList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyMandateList",
                schema: "OrganisationRegistry");
        }
    }
}
