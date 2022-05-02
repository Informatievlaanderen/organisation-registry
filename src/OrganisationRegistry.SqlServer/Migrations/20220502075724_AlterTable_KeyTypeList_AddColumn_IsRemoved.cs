using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AlterTable_KeyTypeList_AddColumn_IsRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                schema: "Backoffice",
                table: "KeyTypeList",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRemoved",
                schema: "Backoffice",
                table: "KeyTypeList");
        }
    }
}
