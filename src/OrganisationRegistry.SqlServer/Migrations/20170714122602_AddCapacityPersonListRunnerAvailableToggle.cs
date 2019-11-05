using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddCapacityPersonListRunnerAvailableToggle : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Toggles:CapacityPersonListRunnerAvailable", "Is 'Personen per hoedanigheid' projectie actief?", "false"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
