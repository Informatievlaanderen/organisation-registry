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
        public virtual OkResult OkWithLocation(string location)
        {
            Response.Headers.Add("Location", location);
            return Ok();
        }
    }
}
