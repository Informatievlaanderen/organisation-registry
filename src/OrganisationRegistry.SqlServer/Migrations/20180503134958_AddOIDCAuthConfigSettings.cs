using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOIDCAuthConfigSettings : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("OIDCAuth:Authority", "OpenID Connect provider", "https://authenticatie-ti.vlaanderen.be/op"));
            //migrationBuilder.Sql(InsertSetting("OIDCAuth:ClientId", "", "30ef5e3e-cdbe-4fa3-95e9-7e18f202342f"));
            //migrationBuilder.Sql(InsertSetting("OIDCAuth:ClientSecret", "", "uzeHuCrCNBfRLJEnKsTNspJJxZC8E1PmVJmV2Ov9nrvay-lErB-JKKkhLMJURfaejrG752x0Zejp8F4hq5n8Y1LMVgi4cKI6"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:CallbackPath", "Callback url (zoals geregistreerd bij Authority)", "/oic"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:TokenEndPoint", "Token endPoint bij Authority", "/v1/token"));

            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtCookieName", "JWT cookie naam.", "twegwijs_api"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtCookieDurationInMinutes", "Leeftijd van authenticatie cookie in minuten.", "1440"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtCookieDomain", "Domein van authenticatie cookie.", ".beta.informatievlaanderen.be"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtCookieSecure", "HTTPS cookie?", "true"));
            //migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtSharedSigningKey", "", "de_weg_is_wijs_man!-fluffybunny123-test"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtIssuer", "JWT issuer", "https://twegwijs-auth.beta.informatievlaanderen.be"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtAudience", "JWT audience.", "https://twegwijs-api.beta.informatievlaanderen.be"));

            migrationBuilder.Sql(InsertSetting("OIDCAuth:Developers", "ACM Ids van ontwikkelaars, gescheiden met punt komma's.", "d8584bf3-4d2a-49c7-8ff2-110bf8cd45d7;fb35efd6-9942-4073-8c8d-99f2c1f79c9b;2c9246c8-ef40-4734-b134-90652ec70acc;92c1e998-0304-4bfd-b017-6d65d442621d"));

            migrationBuilder.Sql(InsertSetting("OIDCAuth:ReturnUrlGuard", "Restrictie op url om terug te sturen na aanmelden.", "https://twegwijs-ui.beta.informatievlaanderen.be"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:SignOutReturnUrl", "Adres om terug te sturen na afmelden.", "https://twegwijs-ui.beta.informatievlaanderen.be"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
