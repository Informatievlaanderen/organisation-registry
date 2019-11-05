using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RenameConfigParamsForReporting : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(DeleteSetting("Api:JuridischeVorm_ClassificationTypeId"));
            migrationBuilder.Sql(InsertSetting("Api:Entiteitsvorm_ClassificationTypeId", "ClassificationType GUID voor 'Entiteitsvorm'", "00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
