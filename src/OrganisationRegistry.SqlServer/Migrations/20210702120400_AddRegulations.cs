using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddRegulations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationRegulationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationRegulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegulationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegulationTypeName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationRegulationList", x => x.OrganisationRegulationId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "RegulationTypeList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulationTypeList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRegulationList_Link",
                schema: "OrganisationRegistry",
                table: "OrganisationRegulationList",
                column: "Link");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRegulationList_RegulationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationRegulationList",
                column: "RegulationTypeName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRegulationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationRegulationList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRegulationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationRegulationList",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_RegulationTypeList_Name",
                schema: "OrganisationRegistry",
                table: "RegulationTypeList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationRegulationList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "RegulationTypeList",
                schema: "OrganisationRegistry");
        }
    }
}
