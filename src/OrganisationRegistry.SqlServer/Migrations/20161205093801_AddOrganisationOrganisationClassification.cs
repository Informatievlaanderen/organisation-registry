using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationOrganisationClassification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationOrganisationClassificationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationOrganisationClassificationId = table.Column<Guid>(nullable: false),
                    OrganisationClassificationId = table.Column<Guid>(nullable: false),
                    OrganisationClassificationName = table.Column<string>(maxLength: 500, nullable: true),
                    OrganisationClassificationTypeId = table.Column<Guid>(nullable: false),
                    OrganisationClassificationTypeName = table.Column<string>(maxLength: 500, nullable: true),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationOrganisationClassificationList", x => x.OrganisationOrganisationClassificationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationOrganisationClassificationList",
                schema: "OrganisationRegistry");
        }
    }
}
