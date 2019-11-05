using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyEventNotifierConfigSettings : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(RenameSetting("VlaanderenBeNotifier:To", "VlaanderenBeNotifier:OrganisationTo"));

            migrationBuilder.Sql(InsertSetting("VlaanderenBeNotifier:BodyTo", "Naar email, gescheiden met punt komma's (organen).", "geert.van.huychem@agiv.be"));
            migrationBuilder.Sql(InsertSetting("VlaanderenBeNotifier:BodyUriTemplate", "Organisatie URI template.", "http://localhost:3000/#/bodies/{0}"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
