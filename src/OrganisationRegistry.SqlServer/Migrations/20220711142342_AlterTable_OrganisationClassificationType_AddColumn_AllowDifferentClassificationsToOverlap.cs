using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AlterTable_OrganisationClassificationType_AddColumn_AllowDifferentClassificationsToOverlap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowDifferentClassificationsToOverlap",
                schema: "Backoffice",
                table: "OrganisationClassificationTypeList",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowDifferentClassificationsToOverlap",
                schema: "Backoffice",
                table: "OrganisationClassificationTypeList");
        }
    }
}
