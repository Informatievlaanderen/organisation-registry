using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateShownOnVlaamseOverheidSitesPerOrganisationList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShowOnVlaamseOverheidSitesPerOrganisationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ShowOnVlaamseOverheidSites = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowOnVlaamseOverheidSitesPerOrganisationList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowOnVlaamseOverheidSitesPerOrganisationList",
                schema: "OrganisationRegistry");
        }
    }
}
