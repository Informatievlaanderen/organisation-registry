using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddConfigurationSettingForReport : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Api:Organisatietype_Mandaten_En_Vermogensaangifte_ClassificationTypeId", "ClassificationType GUID voor 'Organisatietype mandaten- en vermogensaangifte'", "94944afb-7261-554c-dac6-a19ad4387359"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
