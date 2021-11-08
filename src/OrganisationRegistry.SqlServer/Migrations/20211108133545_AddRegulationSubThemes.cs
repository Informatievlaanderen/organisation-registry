using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddRegulationSubThemes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegulationSubThemeList",
                schema: "Backoffice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RegulationThemeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegulationThemeName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulationSubThemeList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegulationSubThemeList_Name",
                schema: "Backoffice",
                table: "RegulationSubThemeList",
                column: "Name")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_RegulationSubThemeList_Name_RegulationThemeId",
                schema: "Backoffice",
                table: "RegulationSubThemeList",
                columns: new[] { "Name", "RegulationThemeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegulationSubThemeList_RegulationThemeName",
                schema: "Backoffice",
                table: "RegulationSubThemeList",
                column: "RegulationThemeName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegulationSubThemeList",
                schema: "Backoffice");
        }
    }
}
