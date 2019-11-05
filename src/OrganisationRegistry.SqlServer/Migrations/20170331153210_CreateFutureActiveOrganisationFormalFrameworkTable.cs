using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateFutureActiveOrganisationFormalFrameworkTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FutureActiveOrganisationFormalFrameworkList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationFormalFrameworkId = table.Column<Guid>(nullable: false),
                    FormalFrameworkId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureActiveOrganisationFormalFrameworkList", x => x.OrganisationFormalFrameworkId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FutureActiveOrganisationFormalFrameworkList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "FutureActiveOrganisationFormalFrameworkList",
                column: "ValidFrom");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FutureActiveOrganisationFormalFrameworkList",
                schema: "OrganisationRegistry");
        }
    }
}
