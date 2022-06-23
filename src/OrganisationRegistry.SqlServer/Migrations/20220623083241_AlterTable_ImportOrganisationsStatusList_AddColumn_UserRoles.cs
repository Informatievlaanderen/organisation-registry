using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AlterTable_ImportOrganisationsStatusList_AddColumn_UserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserRoles",
                schema: "Import",
                table: "ImportOrganisationsStatusList",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserRoles",
                schema: "Import",
                table: "ImportOrganisationsStatusList");
        }
    }
}
