using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddPersonMandates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonMandateList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyMandateId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    BodyName = table.Column<string>(maxLength: 500, nullable: false),
                    BodySeatId = table.Column<Guid>(nullable: false),
                    BodySeatName = table.Column<string>(maxLength: 500, nullable: false),
                    PersonId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonMandateList", x => x.BodyMandateId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonMandateList_BodyName",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                column: "BodyName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonMandateList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_PersonMandateList_ValidTo",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonMandateList",
                schema: "OrganisationRegistry");
        }
    }
}
