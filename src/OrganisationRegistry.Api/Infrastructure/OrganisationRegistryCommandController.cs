namespace OrganisationRegistry.Api.Infrastructure;

using OrganisationRegistry.Infrastructure.Commands;

public class OrganisationRegistryCommandController : OrganisationRegistryController
{
    protected ICommandSender CommandSender { get; }

    public OrganisationRegistryCommandController(ICommandSender commandSender)
    {
        CommandSender = commandSender;
    }
}
