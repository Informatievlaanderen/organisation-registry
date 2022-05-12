namespace OrganisationRegistry.Api.Backoffice.Report.FunctionTypeParticipationReport
{
    using Microsoft.AspNetCore.Mvc;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Commands;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("reports")]
    public class FunctionTypeParticipationController : OrganisationRegistryController
    {
        public FunctionTypeParticipationController(ICommandSender commandSender) : base(commandSender)
        {
        }
    }
}
