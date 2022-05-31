namespace OrganisationRegistry.Infrastructure;

using System.Threading.Tasks;
using Commands;
using Domain;
using Domain.Exception;
using Microsoft.Extensions.Logging;

public class InfrastructureCommandHandlers :
    BaseCommandHandler<InfrastructureCommandHandlers>,
    ICommandEnvelopeHandler<RebuildProjection>
{
    public InfrastructureCommandHandlers(
        ILogger<InfrastructureCommandHandlers> logger,
        ISession session) : base(logger, session)
    { }

    public async Task Handle(ICommandEnvelope<RebuildProjection> envelope)
    {
        try
        {
            var projection = Session.Get<ProjectionRebuilder>(envelope.Command.ProjectionRebuilderId);
            projection.RebuildProjection(envelope.Command.ProjectionName);
        }
        catch (AggregateNotFoundException)
        {
            var projection = new ProjectionRebuilder(envelope.Command.ProjectionRebuilderId);
            Session.Add(projection);

            projection.RebuildProjection(envelope.Command.ProjectionName);
        }

        await Session.Commit(envelope.User);
    }
}
