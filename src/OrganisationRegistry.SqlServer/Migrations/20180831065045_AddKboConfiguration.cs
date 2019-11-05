using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddKboConfiguration : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Api:KboSender", "KBO: identificatie van de afzender", "kb.vlaanderen.be/aiv/organisatieregister-ond-aip"));
            migrationBuilder.Sql(InsertSetting("Api:KboRecipient", "KBO: identificatie van de ontvanger", "vip.vlaanderen.be"));
            migrationBuilder.Sql(InsertSetting("Api:KboMagdaEndpoint", "KBO: endpoint voor MAGDA", "https://magdaondernemingdienst-aip.vlaanderen.be"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
