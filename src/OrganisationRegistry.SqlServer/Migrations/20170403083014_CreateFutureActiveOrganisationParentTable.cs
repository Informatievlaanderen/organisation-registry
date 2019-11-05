using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateFutureActiveOrganisationParentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FutureActiveOrganisationParentList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationOrganisationParentId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ParentOrganisationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureActiveOrganisationParentList", x => x.OrganisationOrganisationParentId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FutureActiveOrganisationParentList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "FutureActiveOrganisationParentList",
                column: "ValidFrom");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FutureActiveOrganisationParentList",
                schema: "OrganisationRegistry");
        }
    }
}
