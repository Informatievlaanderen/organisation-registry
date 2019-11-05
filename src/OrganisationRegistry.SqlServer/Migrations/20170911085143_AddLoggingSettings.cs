using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddLoggingSettings : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Api:EnvironmentName", "Hoe noemt de omgeving?", "Development"));
            migrationBuilder.Sql(InsertSetting("Api:GraphiteAddress", "Op welk adres is Graphite bereikbaar?", "net.tcp://mwegwijsaiv.westeurope.cloudapp.azure.com:2003"));

            migrationBuilder.Sql(InsertSetting("Toggles:EnableMonitoring", "Is monitoring actief?", "false"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
