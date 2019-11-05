using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateOrganisationFormalFrameworkListProjection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationFormalFrameworkList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationFormalFrameworkId = table.Column<Guid>(nullable: false),
                    FormalFrameworkId = table.Column<Guid>(nullable: false),
                    FormalFrameworkName = table.Column<string>(maxLength: 500, nullable: true),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ParentOrganisationId = table.Column<Guid>(nullable: false),
                    ParentOrganisationName = table.Column<string>(maxLength: 500, nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationFormalFrameworkList", x => x.OrganisationFormalFrameworkId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationFormalFrameworkList",
                schema: "OrganisationRegistry");
        }
    }
}
