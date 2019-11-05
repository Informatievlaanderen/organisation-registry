using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodySeatGenderRatioOrganisationCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatio_OrganisationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationId = table.Column<Guid>(nullable: false),
                    OrganisationActive = table.Column<bool>(nullable: false),
                    OrganisationName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatio_OrganisationList", x => x.OrganisationId)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodySeatGenderRatio_OrganisationList",
                schema: "OrganisationRegistry");
        }
    }
}
