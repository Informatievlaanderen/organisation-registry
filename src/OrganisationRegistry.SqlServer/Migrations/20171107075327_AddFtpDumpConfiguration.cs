using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddFtpDumpConfiguration : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Toggles:AgentschapZorgEnGezondheidFtpDumpAvailable", "FTP Dump naar Agentschap Zorg en Gezondheid actief?", "false"));
            migrationBuilder.Sql(InsertSetting("AgentschapZorgEnGezondheidFtpDump:FtpPath", "Folder voor XML Dump.", "/"));
            migrationBuilder.Sql(InsertSetting("AgentschapZorgEnGezondheidFtpDump:XmlLocation", "HTTP Uri naar XML Dump.", "https://api.wegwijs.dev.informatievlaanderen.be:2443/v1/dumps/agentschap-zorg-en-gezondheid/full.xml"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
