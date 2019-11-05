namespace OrganisationRegistry.Api.Infrastructure
{
    using Microsoft.AspNetCore.Mvc;

    public class OrganisationRegistryRouteAttribute : RouteAttribute
    {
        //private const string Prefix = "";
        private const string Prefix = "v{version:apiVersion}/";

        public OrganisationRegistryRouteAttribute(string template) : base(Prefix + template)
        {
        }
    }
}
