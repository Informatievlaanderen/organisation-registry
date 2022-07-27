using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AlterTable_ContactTypeList_AddValidationColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Example",
                schema: "Backoffice",
                table: "ContactTypeList",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Regex",
                schema: "Backoffice",
                table: "ContactTypeList",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ".*");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Example",
                schema: "Backoffice",
                table: "ContactTypeList");

            migrationBuilder.DropColumn(
                name: "Regex",
                schema: "Backoffice",
                table: "ContactTypeList");
        }
    }
}
