using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationRegistrySettings : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Configuration' AND xtype = 'U')
    CREATE TABLE [OrganisationRegistry].[Configuration] (
        [Key] nvarchar(450) NOT NULL,
        [Description] nvarchar(max),
        [Value] nvarchar(max),
        CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED ([Key])
    );");

            //migrationBuilder.CreateTable(
            //   name: "Configuration",
            //   schema: "OrganisationRegistry",
            //   columns: table => new
            //   {
            //       Key = table.Column<string>(maxLength: 450, nullable: false),
            //       Description = table.Column<string>(nullable: true),
            //       Value = table.Column<string>(nullable: true)
            //   },
            //   constraints: table =>
            //   {
            //       table.PrimaryKey("PK_Configuration", x => x.Key)
            //           .Annotation("SqlServer:Clustered", true);
            //   });

            migrationBuilder.Sql(InsertSetting("Toggles:VlaanderenBeNotifierMails", "Stuur vlaanderen.be mails?", "false"));
            migrationBuilder.Sql(InsertSetting("Toggles:VlaanderenBeNotifier", "Is Vlaanderen.be notificaties WebJob actief?", "false"));
	        migrationBuilder.Sql(InsertSetting("Toggles:EnableBodies", "Maak Organen beschikbaar?", "true"));
            migrationBuilder.Sql(InsertSetting("Toggles:ElasticSearchProjections", "Is Elasticsearch WebJob actief?", "false"));
            migrationBuilder.Sql(InsertSetting("Toggles:ElasticSearchLogging", "Log naar Elasticsearch?", "false"));

            migrationBuilder.Sql(InsertSetting("ElasticSearch:LoggingTypeName", "Log Type naam.", "logevent-dev"));
            migrationBuilder.Sql(InsertSetting("ElasticSearch:LoggingIndexFormat", "Log Index formaat.", "logstash-dev-{0:yyyy.MM.dd}"));
            migrationBuilder.Sql(InsertSetting("ElasticSearch:LoggingTemplateName", "Log Template naam.", "serilog-events-template-dev"));
            migrationBuilder.Sql(InsertSetting("ElasticSearch:OrganisationsIndex", "Organisatie Index naam.", "organisations-dev"));
            migrationBuilder.Sql(InsertSetting("ElasticSearch:OrganisationType", "Organisatie Type naam.", "organisation-dev"));

            migrationBuilder.Sql(InsertSetting("ApplicationInsights:InstrumentationKey", "Application Insights InstrumentationKey.", "8a2a29b0-7245-4930-a135-081bf671e910"));

            migrationBuilder.Sql(InsertSetting("Api:CorsOrigin", "CORS Origin.", "https://wegwijs.dev.informatievlaanderen.be:1443"));
            migrationBuilder.Sql(InsertSetting("Api:CorsEnableLocalhost", "Allow localhost in CORS?", "true"));
            migrationBuilder.Sql(InsertSetting("Api:Title", "Titel API.", "OrganisationRegistry API"));
            migrationBuilder.Sql(InsertSetting("Api:Description", "Omschrijving API.", "OrganisationRegistry API - Informatie Vlaanderen"));

            migrationBuilder.Sql(InsertSetting("Auth:JwtCookieName", "JWT cookie naam.", "dwegwijs_api"));
            migrationBuilder.Sql(InsertSetting("Auth:JwtIssuer", "JWT issuer.", "https://auth.wegwijs.dev.informatievlaanderen.be/"));
            migrationBuilder.Sql(InsertSetting("Auth:JwtAudience", "JWT audience.", "https://api.wegwijs.dev.informatievlaanderen.be:2443/"));

            migrationBuilder.Sql(InsertSetting("VlaanderenBeNotifier:SendGridApiUri", "SendGrid API.", "https://api.sendgrid.com/v3/"));
            migrationBuilder.Sql(InsertSetting("VlaanderenBeNotifier:OrganisationUriTemplate", "Organisatie URI template.", "http://localhost:3000/#/organisations/{0}"));
            migrationBuilder.Sql(InsertSetting("VlaanderenBeNotifier:FromName", "Van naam.", "OrganisationRegistry"));
            migrationBuilder.Sql(InsertSetting("VlaanderenBeNotifier:FromAddress", "Van email.", "wegwijs@vlaanderen.be"));
            migrationBuilder.Sql(InsertSetting("VlaanderenBeNotifier:To", "Naar email, gescheiden met punt komma's.", "koen.metsu@agiv.be"));

            migrationBuilder.Sql(InsertSetting("Auth:Endpoint", "Authenticatie url.", "https://auth.wegwijs.dev.informatievlaanderen.be"));
            migrationBuilder.Sql(InsertSetting("Api:Endpoint", "API url.", "https://api.wegwijs.dev.informatievlaanderen.be:2443"));
            migrationBuilder.Sql(InsertSetting("UI:Endpoint", "Frontend url.", "https://wegwijs.dev.informatievlaanderen.be:1443"));

            migrationBuilder.Sql(InsertSetting("Auth:JwtCookieDurationInMinutes", "Leeftijd van authenticatie cookie in minuten.", "1440"));
            migrationBuilder.Sql(InsertSetting("Auth:JwtCookieDomain", "Domein van authenticatie cookie.", "wegwijs.dev.informatievlaanderen.be"));
            migrationBuilder.Sql(InsertSetting("Auth:JwtCookieSecure", "HTTPS cookie?", "true"));

            migrationBuilder.Sql(InsertSetting("Auth:ReturnUrlGuard", "Restrictie op url om terug te sturen na aanmelden.", "wegwijs.dev.informatievlaanderen.be"));
            migrationBuilder.Sql(InsertSetting("Auth:SignOutReturnUrl", "Adres om terug te sturen na afmelden.", "https://wegwijs.dev.informatievlaanderen.be:1443"));

            migrationBuilder.Sql(InsertSetting("Auth:Developers", "ACM Ids van ontwikkelaars, gescheiden met punt komma's.", "d8584bf3-4d2a-49c7-8ff2-110bf8cd45d7;fb35efd6-9942-4073-8c8d-99f2c1f79c9b;2c9246c8-ef40-4734-b134-90652ec70acc;92c1e998-0304-4bfd-b017-6d65d442621d"));

            migrationBuilder.Sql(InsertSetting("Auth:STSRealm", "Authenticatie realm.", "urn:vlaanderen.be/wegwijs/dev"));
            migrationBuilder.Sql(InsertSetting("Auth:STSHost", "Authenticate hostname.", "auth.wegwijs.dev.informatievlaanderen.be"));
            migrationBuilder.Sql(InsertSetting("Auth:STSIssuer", "Authenticate issuer.", "https://auth.wegwijs.dev.informatievlaanderen.be"));
            migrationBuilder.Sql(InsertSetting("Auth:STSRequireSSL", "HTTPS verplicht bij authenticatie?", "true"));
            migrationBuilder.Sql(InsertSetting("Auth:STSCookie", "Authenticate cookie naam.", ".SALVAUTH"));

            migrationBuilder.Sql(InsertSetting("Auth:IdPRealm", "Identity Provider realm.", "https://authenticatie-ti.vlaanderen.be/sps/vidp/saml20"));
            migrationBuilder.Sql(InsertSetting("Auth:SamlpSingleLogoutServiceResponseUrl", "ACM SAML Logout Url.", "https://authenticatie-ti.vlaanderen.be/sps/vidp/saml20/slo"));
            migrationBuilder.Sql(InsertSetting("Auth:IssuerUrl", "ACM SAML Login Url.", "https://authenticatie-ti.vlaanderen.be/sps/vidp/saml20/login"));

            migrationBuilder.Sql(InsertSetting("Auth:SigningCertificateFile", "OrganisationRegistry Signing Certificate.", "dwegwijs.serviceprovider.informatievlaanderen.be.pfx"));
            migrationBuilder.Sql(InsertSetting("Auth:IssuerCertificateFile", "ACM Signing Certificate.", "vidp-signing.cer"));

            migrationBuilder.Sql(InsertSetting("Toggles:ApplicationOnline", "Is OrganisationRegistry beschikbaar?", "true"));

            migrationBuilder.Sql(DeleteSetting("Toggles:VlaanderenBeNotifierMails"));
        	migrationBuilder.Sql(DeleteSetting("Toggles:VlaanderenBeNotifier"));
            migrationBuilder.Sql(DeleteSetting("Toggles:ElasticSearchProjections"));
            migrationBuilder.Sql(DeleteSetting("Toggles:ElasticSearchLogging"));
            migrationBuilder.Sql(DeleteSetting("Toggles:ApplicationOnline"));

            migrationBuilder.Sql(InsertSetting("Toggles:SendVlaanderenBeNotifierMails", "Stuur vlaanderen.be mails?", "false"));
            migrationBuilder.Sql(InsertSetting("Toggles:VlaanderenBeNotifierAvailable", "Is Vlaanderen.be notificaties WebJob actief?", "false"));
            migrationBuilder.Sql(InsertSetting("Toggles:ElasticSearchProjectionsAvailable", "Is Elasticsearch WebJob actief?", "false"));
            migrationBuilder.Sql(InsertSetting("Toggles:LogToElasticSearch", "Log naar Elasticsearch?", "false"));
            migrationBuilder.Sql(InsertSetting("Toggles:ApplicationAvailable", "Is OrganisationRegistry beschikbaar?", "true"));
            migrationBuilder.Sql(InsertSetting("Toggles:ApiAvailable", "Is OrganisationRegistry API beschikbaar?", "true"));

            migrationBuilder.Sql(InsertSetting("ElasticSearch:LoggingNumberOfShards", "Aantal shards voor serilog.", "1"));

            migrationBuilder.Sql(InsertSetting("ElasticSearchJanitor:NumberOfDaysBeforeIndexesAreClosed", "Na hoeveel dagen een ElasticSearch Janitor time-based indices sluit. (min: 7)", "7"));
            migrationBuilder.Sql(InsertSetting("Toggles:ElasticSearchJanitorAvailable", "Is Elasticsearch Janitor webjob actief?", "false"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "Configuration",
               schema: "OrganisationRegistry");
        }
    }
}
