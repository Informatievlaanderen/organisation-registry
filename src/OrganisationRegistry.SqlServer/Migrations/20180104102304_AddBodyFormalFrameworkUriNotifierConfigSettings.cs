using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyFormalFrameworkUriNotifierConfigSettings : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("VlaanderenBeNotifier:BodyFormalFrameworkUriTemplate", "Toepassingsgebieden URI template.", "http://localhost:3000/#/bodies/{0}/formalframeworks"));
            migrationBuilder.Sql(InsertSetting("VlaanderenBeNotifier:MepFormalFrameworkId", "FormalFramework GUID voor MEP.", "00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
