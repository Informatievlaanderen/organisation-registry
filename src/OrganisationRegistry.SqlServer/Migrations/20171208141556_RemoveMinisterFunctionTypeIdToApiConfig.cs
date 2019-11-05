using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RemoveMinisterFunctionTypeIdToApiConfig : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(DeleteSetting("Api:MinisterFunctionTypeId"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
