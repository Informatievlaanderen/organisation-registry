using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddEmailContactTypeId : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Api:EmailContactTypeId", "ContactType GUID voor e-mail", "00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(DeleteSetting("Toggles:CapacityPersonListRunnerAvailable"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
