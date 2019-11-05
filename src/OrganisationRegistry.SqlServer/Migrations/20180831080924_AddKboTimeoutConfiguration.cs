using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddKboTimeoutConfiguration : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Api:KboMagdaTimeout", "KBO: tiemout voor MAGDA", "10"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
