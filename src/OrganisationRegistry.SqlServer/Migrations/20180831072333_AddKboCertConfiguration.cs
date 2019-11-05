using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddKboCertConfiguration : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Api:KboCertificate", "KBO: identificatie van de afzender", @"wegwijs-aip.pfx"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
