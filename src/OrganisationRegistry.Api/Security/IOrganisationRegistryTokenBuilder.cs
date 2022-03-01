namespace OrganisationRegistry.Api.Security
{
    using System.Security.Claims;

    public interface IOrganisationRegistryTokenBuilder
    {
        string BuildJwt(ClaimsIdentity identity);

        ClaimsIdentity ParseRoles(ClaimsIdentity identity);
    }
}
