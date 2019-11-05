using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddInformatieVlaanderenLinkConfig : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("UI:InformatieVlaanderenLink", "Link naar Informatie Vlaanderen", "https://www.vlaanderen.be/informatievlaanderen"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
