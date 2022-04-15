namespace OrganisationRegistry.Api.Infrastructure
{
    using Microsoft.AspNetCore.Mvc;
    using OrganisationRegistry.Infrastructure.Commands;

    public class OrganisationRegistryController : Controller
    {
        protected ICommandSender CommandSender { get; }

        public OrganisationRegistryController(ICommandSender commandSender)
        {
            CommandSender = commandSender;
        }

        [NonAction]
        protected OkResult OkWithLocationHeader(string action, object? parameters)
        {
            var maybeLocationHeader = Url.Action(action, parameters);
            if (maybeLocationHeader is not { } locationHeader)
                throw new ApiException($"Action {action} does not exist");

            Response.Headers.Add("Location", locationHeader);
            return Ok();
        }

        [NonAction]
        protected CreatedResult CreatedWithLocation(string action, object? parameters)
        {
            var maybeLocationHeader = Url.Action(action, parameters);
            if (maybeLocationHeader is not { } locationHeader)
                throw new ApiException($"Action {action} does not exist");

            return Created(locationHeader, null);
        }
    }
}
