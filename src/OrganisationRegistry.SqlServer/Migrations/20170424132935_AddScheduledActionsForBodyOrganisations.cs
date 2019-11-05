using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddScheduledActionsForBodyOrganisations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveBodyOrganisationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyOrganisationId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveBodyOrganisationList", x => x.BodyOrganisationId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "FutureActiveBodyOrganisationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyOrganisationId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureActiveBodyOrganisationList", x => x.BodyOrganisationId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveBodyOrganisationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "ActiveBodyOrganisationList",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_FutureActiveBodyOrganisationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "FutureActiveBodyOrganisationList",
                column: "ValidFrom");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveBodyOrganisationList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "FutureActiveBodyOrganisationList",
                schema: "OrganisationRegistry");
        }
    }
}
