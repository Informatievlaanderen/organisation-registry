using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddContactLabelOrganisationRelationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactTypeList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactTypeList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LabelTypeList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabelTypeList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationRelationTypeList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationRelationTypeList", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactTypeList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "LabelTypeList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "OrganisationRelationTypeList",
                schema: "OrganisationRegistry");
        }
    }
}
