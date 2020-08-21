using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyClassificationTypeConfigurationData : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                InsertSetting(
                    "Api:BodyPolicyDomainClassificationTypeId",
                    "Orgaan Classificatietype GUID voor 'Beleidsdomein'",
                    "00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(
                InsertSetting(
                    "Api:BodyResponsibleMinisterClassificationTypeId",
                    "Orgaan Classificatietype GUID voor 'Bevoegde minister'",
                    "00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
