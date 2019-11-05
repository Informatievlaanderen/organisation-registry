using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddActiveOrganisationBuildingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveOrganisationBuildingList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationBuildingId = table.Column<Guid>(nullable: false),
                    BuildingId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveOrganisationBuildingList", x => x.OrganisationBuildingId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveOrganisationBuildingList_ValidTo",
                schema: "OrganisationRegistry",
                table: "ActiveOrganisationBuildingList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveOrganisationBuildingList",
                schema: "OrganisationRegistry");
        }
    }
}
