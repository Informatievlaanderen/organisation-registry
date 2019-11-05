using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddEnableReportingToggles : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Toggles:EnableReporting", "Enable reporting tab?", "false"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
