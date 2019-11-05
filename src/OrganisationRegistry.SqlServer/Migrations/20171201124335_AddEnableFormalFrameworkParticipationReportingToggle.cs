using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddEnableFormalFrameworkParticipationReportingToggle : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Toggles:EnableFormalFrameworkParticipationReporting", "Is rapportering participatie per toepassingsgebied actief?", "true"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
