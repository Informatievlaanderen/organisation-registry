using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddFormalFrameworOrganisationsVademecumFilter : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Api:VademecumReport_FormalFrameworkIds", "FormalFramework GUIDs voor Vademecum rapport", "00000000-0000-0000-0000-000000000000"));

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
