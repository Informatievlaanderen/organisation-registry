using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AlterTable_OrganisationCapacityList_AddColumn_ScheduledForRemoval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ScheduledForRemoval",
                schema: "Backoffice",
                table: "OrganisationCapacityList",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledForRemoval",
                schema: "Backoffice",
                table: "OrganisationCapacityList");
        }
    }
}
