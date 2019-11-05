using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationParentList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationParentList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationOrganisationParentId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ParentOrganisationId = table.Column<Guid>(nullable: false),
                    ParentOrganisationName = table.Column<string>(maxLength: 500, nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationParentList", x => x.OrganisationOrganisationParentId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationParentList",
                schema: "OrganisationRegistry");
        }
    }
}
