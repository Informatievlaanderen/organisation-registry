using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationSecurityTree : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationTreeList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OvoNumber = table.Column<string>(maxLength: 10, nullable: false),
                    OrganisationTree = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationTreeList", x => x.OvoNumber);
                });

            migrationBuilder.AlterColumn<string>(
                name: "OvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationTreeList",
                schema: "OrganisationRegistry");

            migrationBuilder.AlterColumn<string>(
                name: "OvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);
        }
    }
}
