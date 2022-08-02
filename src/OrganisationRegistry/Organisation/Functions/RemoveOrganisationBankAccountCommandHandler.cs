namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class RemoveOrganisationFunctionCommandHandler
    : BaseCommandHandler<RemoveOrganisationFunctionCommandHandler>
        , ICommandEnvelopeHandler<RemoveOrganisationFunction>
{
    public RemoveOrganisationFunctionCommandHandler(ILogger<RemoveOrganisationFunctionCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public async Task Handle(ICommandEnvelope<RemoveOrganisationFunction> envelope)
        => await UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.Id);
                    organisation.RemoveFunction(envelope.Command.OrganisationFunctionId);
                });
}
