using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddWorRulesUrlToOrganisationRegulation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkRulesUrl",
                schema: "Backoffice",
                table: "OrganisationRegulationList",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkRulesUrl",
                schema: "Backoffice",
                table: "OrganisationRegulationList");
        }
    }
}
