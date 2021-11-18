using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddRegulationNameAndRenameProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Link",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                newName: "Url");

            migrationBuilder.RenameIndex(
                name: "IX_OrganisationRegulationList_Link",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                newName: "IX_OrganisationRegulationList_Url");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "Backoffice",
                table: "OrganisationRegulationList");

            migrationBuilder.RenameColumn(
                name: "Url",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                newName: "Link");

            migrationBuilder.RenameIndex(
                name: "IX_OrganisationRegulationList_Url",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                newName: "IX_OrganisationRegulationList_Link");
        }
    }
}
