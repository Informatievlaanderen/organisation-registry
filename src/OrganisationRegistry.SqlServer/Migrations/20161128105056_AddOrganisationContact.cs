using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationContact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationContactList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationContactId = table.Column<Guid>(nullable: false),
                    ContactTypeId = table.Column<Guid>(nullable: false),
                    ContactTypeName = table.Column<string>(maxLength: 500, nullable: true),
                    ContactValue = table.Column<string>(maxLength: 500, nullable: true),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationContactList", x => x.OrganisationContactId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationContactList",
                schema: "OrganisationRegistry");
        }
    }
}
