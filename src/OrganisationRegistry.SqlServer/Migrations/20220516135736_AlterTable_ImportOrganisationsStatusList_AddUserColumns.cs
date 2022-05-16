using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AlterTable_ImportOrganisationsStatusList_AddUserColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "User",
                schema: "Import",
                table: "ImportOrganisationsStatusList",
                newName: "UserName");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Import",
                table: "ImportOrganisationsStatusList",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Import",
                table: "ImportOrganisationsStatusList");

            migrationBuilder.RenameColumn(
                name: "UserName",
                schema: "Import",
                table: "ImportOrganisationsStatusList",
                newName: "User");
        }
    }
}
