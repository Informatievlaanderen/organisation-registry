using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateBodyOrganisationListTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyOrganisationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyOrganisationId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    OrganisationName = table.Column<string>(maxLength: 500, nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyOrganisationList", x => x.BodyOrganisationId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodyOrganisationList_OrganisationName",
                schema: "OrganisationRegistry",
                table: "BodyOrganisationList",
                column: "OrganisationName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyOrganisationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyOrganisationList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_BodyOrganisationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "BodyOrganisationList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyOrganisationList",
                schema: "OrganisationRegistry");
        }
    }
}
