using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddElasticBodyIndices : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("ElasticSearch:BodyWriteIndex", "Orgaan Write Index naam.", "body-dev"));
            migrationBuilder.Sql(InsertSetting("ElasticSearch:BodyReadIndex", "Orgaan Read Index naam.", "body-dev"));
            migrationBuilder.Sql(InsertSetting("ElasticSearch:BodyType", "Orgaan Write Index naam.", "body-dev"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
