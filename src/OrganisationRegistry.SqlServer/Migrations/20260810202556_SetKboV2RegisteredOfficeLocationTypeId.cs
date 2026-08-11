using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class SetKboV2RegisteredOfficeLocationTypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [OrganisationRegistry].[Configuration]
                SET [Value] = 'a7e93f04-0004-0000-0000-000000000001'
                WHERE [Key] = 'Api:KboV2RegisteredOfficeLocationTypeId'
                  AND [Value] = '00000000-0000-0000-0000-000000000000';

                UPDATE [OrganisationRegistry].[Configuration]
                SET [Value] = 'a7e93f06-0006-0000-0000-000000000001'
                WHERE [Key] = 'Api:KboV2LegalFormOrganisationClassificationTypeId'
                  AND [Value] = '00000000-0000-0000-0000-000000000000';

                UPDATE [OrganisationRegistry].[Configuration]
                SET [Value] = 'a7e93f02-0002-0000-0000-000000000004'
                WHERE [Key] = 'Api:KboV2FormalNameLabelTypeId'
                  AND [Value] = '00000000-0000-0000-0000-000000000000';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [OrganisationRegistry].[Configuration]
                SET [Value] = '00000000-0000-0000-0000-000000000000'
                WHERE [Key] IN (
                    'Api:KboV2RegisteredOfficeLocationTypeId',
                    'Api:KboV2LegalFormOrganisationClassificationTypeId',
                    'Api:KboV2FormalNameLabelTypeId'
                )
                  AND [Value] IN (
                    'a7e93f04-0004-0000-0000-000000000001',
                    'a7e93f06-0006-0000-0000-000000000001',
                    'a7e93f02-0002-0000-0000-000000000004'
                  );
                """);
        }
    }
}
