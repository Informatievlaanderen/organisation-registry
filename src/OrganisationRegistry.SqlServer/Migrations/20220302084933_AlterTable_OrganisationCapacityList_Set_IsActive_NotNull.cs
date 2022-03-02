using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AlterTable_OrganisationCapacityList_Set_IsActive_NotNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE [Backoffice].[OrganisationCapacityList]
SET IsActive = 0
WHERE IsActive IS null
");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Backoffice",
                table: "OrganisationCapacityList",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Backoffice",
                table: "OrganisationCapacityList",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
