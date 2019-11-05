using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddPhoneAndCellPhoneContactTypeIdToApiConf : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Api:PhoneContactTypeId", "ContactType GUID voor telefoon", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:CellPhoneContactTypeId", "ContactType GUID voor gsm", "00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
