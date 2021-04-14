using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationsToRebuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ElasticSearchProjections");

            migrationBuilder.RenameTable(
                name: "ShowOnVlaamseOverheidSitesPerOrganisationList",
                schema: "OrganisationRegistry",
                newName: "ShowOnVlaamseOverheidSitesPerOrganisationList",
                newSchema: "ElasticSearchProjections");

            migrationBuilder.RenameTable(
                name: "OrganisationPerBodyListForES",
                schema: "OrganisationRegistry",
                newName: "OrganisationPerBodyListForES",
                newSchema: "ElasticSearchProjections");

            migrationBuilder.RenameTable(
                name: "IsActivePerOrganisationCapacityList",
                schema: "OrganisationRegistry",
                newName: "IsActivePerOrganisationCapacityList",
                newSchema: "ElasticSearchProjections");

            migrationBuilder.CreateTable(
                name: "OrganisationsToRebuild",
                schema: "ElasticSearchProjections",
                columns: table => new
                {
                    OrganisationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationsToRebuild", x => x.OrganisationId)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationsToRebuild",
                schema: "ElasticSearchProjections");

            migrationBuilder.RenameTable(
                name: "ShowOnVlaamseOverheidSitesPerOrganisationList",
                schema: "ElasticSearchProjections",
                newName: "ShowOnVlaamseOverheidSitesPerOrganisationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationPerBodyListForES",
                schema: "ElasticSearchProjections",
                newName: "OrganisationPerBodyListForES",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "IsActivePerOrganisationCapacityList",
                schema: "ElasticSearchProjections",
                newName: "IsActivePerOrganisationCapacityList",
                newSchema: "OrganisationRegistry");
        }
    }
}
