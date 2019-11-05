using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RenameDescriptionBodyUriTemplate : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                RenameSettingDescription(
                    "VlaanderenBeNotifier:BodyUriTemplate",
                    "Orgaan URI template."));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
