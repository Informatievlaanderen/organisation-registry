using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddActiveOrganisationParentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveOrganisationParentList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationOrganisationParentId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ParentOrganisationId = table.Column<Guid>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveOrganisationParentList", x => x.OrganisationOrganisationParentId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveOrganisationParentList_ValidTo",
                schema: "OrganisationRegistry",
                table: "ActiveOrganisationParentList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveOrganisationParentList",
                schema: "OrganisationRegistry");
        }
    }
}
