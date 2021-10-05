namespace OrganisationRegistry.Api.Infrastructure
{
    using Be.Vlaanderen.Basisregisters.Api;

    public class OrganisationRegistryRouteAttribute : ApiRouteAttribute
    {
        public OrganisationRegistryRouteAttribute(string template) : base(template)
        {
        }
    }
}
