using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationLabelList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationLabelList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationLabelId = table.Column<Guid>(nullable: false),
                    LabelTypeId = table.Column<Guid>(nullable: false),
                    LabelTypeName = table.Column<string>(maxLength: 500, nullable: true),
                    LabelValue = table.Column<string>(maxLength: 500, nullable: true),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationLabelList", x => x.OrganisationLabelId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationLabelList",
                schema: "OrganisationRegistry");
        }
    }
}
