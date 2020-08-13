using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddRepertoriumConfigurationData : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                InsertSetting(
                    "Api:RepertoriumMagdaEndpoint",
                    "Repertorium: endpoint voor Reportorium inschrijvingen",
                    "https://magdarepertoriumdienst-aip.vlaanderen.be"));

            migrationBuilder.Sql(
                InsertSetting(
                    "Api:RepertoriumCapacity",
                    "Repertorium: hoedanigheid voor Reportorium inschrijvingen",
                    "7001"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
