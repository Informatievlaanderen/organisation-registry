namespace OrganisationRegistry.Api.Infrastructure;

using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

public class OrganisationRegistryCommandController : OrganisationRegistryController
{
    protected ICommandSender CommandSender { get; }

    public OrganisationRegistryCommandController(ICommandSender commandSender)
    {
        CommandSender = commandSender;
    }

    [NonAction]
    public override bool TryValidateModel(object model)
        => ModelState.IsValid && base.TryValidateModel(model);
}
