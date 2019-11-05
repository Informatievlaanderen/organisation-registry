using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddEnableFunctionTypeReportingToggle : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Toggles:EnableFunctionTypeParticipationReporting", "Is rapportering participatie per minsterpost actief?", "false"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
