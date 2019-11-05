using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddClassificationTypeIdsForReporting : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Api:LegalFormClassificationTypeId", "Classificatietype GUID voor 'Juridische vorm'", "00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(InsertSetting("Api:PolicyDomainClassificationTypeId", "Classificatietype GUID voor 'Beleidsdomein'", "00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(InsertSetting("Api:ResponsibleMinisterClassificationTypeId", "Classificatietype GUID voor 'Bevoegde minister'", "00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(InsertSetting("Api:DataVlaanderenOrganisationUri", "Organisatie Uri naar data.vlaanderen.be", "http://data.vlaanderen.be/doc/organisaties/{0}"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
