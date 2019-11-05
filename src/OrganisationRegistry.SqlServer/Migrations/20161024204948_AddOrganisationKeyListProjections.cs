using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationKeyListProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationKeyList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationId = table.Column<Guid>(nullable: false),
                    KeyId = table.Column<Guid>(nullable: false),
                    KeyValue = table.Column<string>(maxLength: 500, nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationKeyList", x => new { x.OrganisationId, x.KeyId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationKeyList",
                schema: "OrganisationRegistry");
        }
    }
}
