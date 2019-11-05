using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddLabelTypeIds : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Api:FrenchLabelTypeId", "LabelType GUID voor Frans", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:GermanLabelTypeId", "LabelType GUID voor Duits", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:EnglishLabelTypeId", "LabelType GUID voor Engels", "00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
