using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationClassificationTypeProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationClassificationTypeList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationClassificationTypeList", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationClassificationTypeList",
                schema: "OrganisationRegistry");
        }
    }
}
