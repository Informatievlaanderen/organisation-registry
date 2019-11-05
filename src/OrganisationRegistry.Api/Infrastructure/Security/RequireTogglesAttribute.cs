namespace OrganisationRegistry.Api.Infrastructure.Security
{
    using System;
    using System.Linq;
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Configuration;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireTogglesAttribute : Attribute, IResourceFilter
    {
        private readonly string[] _requiredToggles;

        public RequireTogglesAttribute(params string[] requiredToggles)
        {
            _requiredToggles = requiredToggles;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var togglesConfiguration = context.HttpContext.RequestServices.GetService<IOptions<TogglesConfiguration>>().Value;

            var allToggles =
                _requiredToggles
                    .All(desiredToggle =>
                        togglesConfiguration.GetType().GetProperty(desiredToggle) != null &&
                        (bool) togglesConfiguration.GetType().GetProperty(desiredToggle).GetValue(togglesConfiguration));

            if (!allToggles)
                context.Result = new StatusCodeResult((int) HttpStatusCode.ServiceUnavailable);
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }
    }
}
