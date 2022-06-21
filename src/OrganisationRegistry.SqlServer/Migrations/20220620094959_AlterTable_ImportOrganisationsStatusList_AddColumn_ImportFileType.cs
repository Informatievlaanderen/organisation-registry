using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AlterTable_ImportOrganisationsStatusList_AddColumn_ImportFileType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImportFileType",
                schema: "Import",
                table: "ImportOrganisationsStatusList",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "create");

            migrationBuilder.AlterColumn<string>(
                name: "ImportFileType",
                schema: "Import",
                table: "ImportOrganisationsStatusList",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportFileType",
                schema: "Import",
                table: "ImportOrganisationsStatusList");
        }
    }
}
