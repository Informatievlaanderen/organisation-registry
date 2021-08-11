namespace OrganisationRegistry.Infrastructure
{
    using System.Threading.Tasks;
    using Commands;
    using Domain;
    using Domain.Exception;
    using Microsoft.Extensions.Logging;

    public class InfrastructureCommandHandlers :
        BaseCommandHandler<InfrastructureCommandHandlers>,
        ICommandHandler<RebuildProjection>
    {
        public InfrastructureCommandHandlers(
            ILogger<InfrastructureCommandHandlers> logger,
            ISession session) : base(logger, session)
        { }

        public async Task Handle(RebuildProjection message)
        {
            try
            {
                var projection = Session.Get<ProjectionRebuilder>(message.ProjectionRebuilderId);
                projection.RebuildProjection(message.ProjectionName);
            }
            catch (AggregateNotFoundException)
            {
                var projection = new ProjectionRebuilder(message.ProjectionRebuilderId);
                Session.Add(projection);

                projection.RebuildProjection(message.ProjectionName);
            }

            await Session.Commit(message.User);
        }
    }
}
