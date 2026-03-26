using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOIDCAuthConfigSettings : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("OIDCAuth:Authority", "OpenID Connect provider", "http://keycloak/realms/wegwijs"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:AuthorizationIssuer", "OpenID Connect issuer", "http://keycloak.localhost:9080/realms/wegwijs"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:AuthorizationEndpoint", "OpenID Connect authorization endpoint", "http://keycloak.localhost:9080/realms/wegwijs/protocol/openid-connect/auth"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:UserInfoEndPoint", "OpenID Connect userinfo endpoint", "http://keycloak.localhost:9080/realms/wegwijs/protocol/openid-connect/userinfo"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:EndSessionEndPoint", "OpenID Connect end session endpoint", "http://keycloak.localhost:9080/realms/wegwijs/protocol/openid-connect/logout"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwksUri", "OpenID Connect JWKS URI", "http://keycloak/realms/wegwijs/protocol/openid-connect/certs"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:ClientId", "OpenID Connect client id", "organisation-registry-local-dev"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:ClientSecret", "OpenID Connect client secret", "a_very=Secr3t*Key"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:CallbackPath", "Callback url (zoals geregistreerd bij Authority)", "/oic"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:TokenEndPoint", "Token endPoint bij Authority", "/protocol/openid-connect/token"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:AuthorizationRedirectUri", "Redirect URI na authorisatie", "http://ui.localhost:9080/oic"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:PostLogoutRedirectUri", "Redirect URI na logout", "http://ui.localhost:9080"));

            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtCookieName", "JWT cookie naam.", "twegwijs_api"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtCookieDurationInMinutes", "Leeftijd van authenticatie cookie in minuten.", "1440"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtCookieDomain", "Domein van authenticatie cookie.", ".beta.informatievlaanderen.be"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtCookieSecure", "HTTPS cookie?", "true"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtSharedSigningKey", "JWT signing key.", "keycloak-demo-local-dev-secret-key-32b"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtIssuer", "JWT issuer", "organisatieregister"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtAudience", "JWT audience.", "organisatieregister"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:JwtExpiresInMinutes", "JWT verloopduur in minuten.", "120"));

            migrationBuilder.Sql(InsertSetting("OIDCAuth:Developers", "ACM Ids van ontwikkelaars, gescheiden met punt komma's.", "d8584bf3-4d2a-49c7-8ff2-110bf8cd45d7;fb35efd6-9942-4073-8c8d-99f2c1f79c9b;2c9246c8-ef40-4734-b134-90652ec70acc;92c1e998-0304-4bfd-b017-6d65d442621d"));

            migrationBuilder.Sql(InsertSetting("OIDCAuth:ReturnUrlGuard", "Restrictie op url om terug te sturen na aanmelden.", "https://twegwijs-ui.beta.informatievlaanderen.be"));
            migrationBuilder.Sql(InsertSetting("OIDCAuth:SignOutReturnUrl", "Adres om terug te sturen na afmelden.", "https://twegwijs-ui.beta.informatievlaanderen.be"));

            migrationBuilder.Sql(InsertSetting("EditApi:ClientId", "Edit API client id", "organisation-registry-api"));
            migrationBuilder.Sql(InsertSetting("EditApi:ClientSecret", "Edit API client secret", "a_very=Secr3t*Key"));
            migrationBuilder.Sql(InsertSetting("EditApi:Authority", "Edit API authority", "http://localhost:8180/realms/wegwijs"));
            migrationBuilder.Sql(InsertSetting("EditApi:IntrospectionEndpoint", "Edit API introspection endpoint", "http://localhost:8180/realms/wegwijs/protocol/openid-connect/token/introspect"));

            migrationBuilder.Sql(InsertSetting("FeatureManagement:EditApi", "Edit API feature toggle", "true"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
