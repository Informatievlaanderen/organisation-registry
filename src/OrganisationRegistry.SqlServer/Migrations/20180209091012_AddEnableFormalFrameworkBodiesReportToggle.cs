using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddEnableFormalFrameworkBodiesReportToggle : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Toggles:EnableFormalFrameworkBodiesReporting", "Is rapportering organen per toepassingsgebied actief?", "false"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
