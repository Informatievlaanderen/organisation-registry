using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddKboV2ConfigurationSettings : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Api:KboV2RegisteredOfficeLocationTypeId", "KBO-veld v2: LocationType Id voor Maatschappelijke zetel", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:KboV2LegalFormOrganisationClassificationTypeId", "KBO-veld v2: OrganisationClassificationType Id voor Rechtsvorm", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:KboV2FormalNameLabelTypeId", "KBO-veld v2: LabelType Id voor Formele Benaming", "00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(DeleteSetting("Api:KboV2RegisteredOfficeLocationTypeId"));
            migrationBuilder.Sql(DeleteSetting("Api:KboV2LegalFormOrganisationClassificationTypeId"));
            migrationBuilder.Sql(DeleteSetting("Api:KboV2FormalNameLabelTypeId"));
        }
    }
}
