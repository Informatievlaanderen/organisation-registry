using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationBodyListView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationBodyList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationBodyId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    BodyName = table.Column<string>(maxLength: 500, nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationBodyList", x => x.OrganisationBodyId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationBodyList_BodyName",
                schema: "OrganisationRegistry",
                table: "OrganisationBodyList",
                column: "BodyName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationBodyList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationBodyList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationBodyList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationBodyList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationBodyList",
                schema: "OrganisationRegistry");
        }
    }
}
