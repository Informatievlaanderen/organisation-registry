using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OrganisationRegistry.SqlServer.Infrastructure;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(OrganisationRegistryContext))]
    [Migration("20260430103600_RemoveStaleOidcConfigurationKeys")]
    public partial class RemoveStaleOidcConfigurationKeys : BaseMigration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // DB-backed config provider wordt LAATST geladen in de API en zou env vars overriden
            // Verwijder stale OIDCAuth rijen zodat environment variables gebruikt worden
            var staleKeys = new[]
            {
                "OIDCAuth:JwtAudience",
                "OIDCAuth:JwtIssuer", 
                "OIDCAuth:JwksUri",
                "OIDCAuth:Authority",
                "OIDCAuth:IntrospectionEndpoint"
            };

            foreach (var staleKey in staleKeys)
            {
                migrationBuilder.Sql(
                    $"DELETE FROM [OrganisationRegistry].[Configuration] WHERE [Key] = '{staleKey}'");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
