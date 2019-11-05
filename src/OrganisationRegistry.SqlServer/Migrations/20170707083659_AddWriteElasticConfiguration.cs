using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddWriteElasticConfiguration : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("ElasticSearch:OrganisationsWriteIndex", "Organisatie Write Index naam.", "organisations-dev"));
            migrationBuilder.Sql(RenameSetting("ElasticSearch:OrganisationsIndex", "ElasticSearch:OrganisationsReadIndex"));

            migrationBuilder.Sql(InsertSetting("ElasticSearch:PeopleWriteIndex", "Persoon Write Index naam.", "people-dev"));
            migrationBuilder.Sql(RenameSetting("ElasticSearch:PeopleIndex", "ElasticSearch:PeopleReadIndex"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
