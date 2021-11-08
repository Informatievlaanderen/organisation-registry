using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RenameRegulationTypeToTheme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegulationTypeList",
                schema: "Backoffice");

            migrationBuilder.RenameColumn(
                name: "RegulationTypeName",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                newName: "RegulationThemeName");

            migrationBuilder.RenameColumn(
                name: "RegulationTypeId",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                newName: "RegulationThemeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganisationRegulationList_RegulationTypeName",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                newName: "IX_OrganisationRegulationList_RegulationThemeName");

            migrationBuilder.CreateTable(
                name: "RegulationThemeList",
                schema: "Backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulationThemeList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegulationThemeList_Name",
                schema: "Backoffice",
                table: "RegulationThemeList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegulationThemeList",
                schema: "Backoffice");

            migrationBuilder.RenameColumn(
                name: "RegulationThemeName",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                newName: "RegulationTypeName");

            migrationBuilder.RenameColumn(
                name: "RegulationThemeId",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                newName: "RegulationTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganisationRegulationList_RegulationThemeName",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                newName: "IX_OrganisationRegulationList_RegulationTypeName");

            migrationBuilder.CreateTable(
                name: "RegulationTypeList",
                schema: "Backoffice",
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
                name: "IX_RegulationTypeList_Name",
                schema: "Backoffice",
                table: "RegulationTypeList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }
    }
}
