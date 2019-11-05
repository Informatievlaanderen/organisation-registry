using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddPeopleIndex : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("ElasticSearch:PeopleIndex", "Persoon Index naam.", "people-dev"));
            migrationBuilder.Sql(InsertSetting("ElasticSearch:PersonType", "Persoon Type naam.", "person-dev"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
