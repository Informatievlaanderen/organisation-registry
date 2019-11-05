using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddReportingRunnerToggle : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Toggles:ReportingRunnerAvailable", "Is Rapportering projectie actief?", "false"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
