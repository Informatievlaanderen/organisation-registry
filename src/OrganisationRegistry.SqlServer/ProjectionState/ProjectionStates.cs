namespace OrganisationRegistry.SqlServer.ProjectionState
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Infrastructure;

    public class ProjectionStates : IProjectionStates
    {
        private readonly IContextFactory _contextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ProjectionStates(IContextFactory contextFactory, IDateTimeProvider dateTimeProvider)
        {
            _contextFactory = contextFactory;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<int> GetLastProcessedEventNumber(string projectionName)
        {
            await using var context = _contextFactory.Create();
            var state =
                context.ProjectionStates
                    .SingleOrDefault(item => item.Name == projectionName);

            if (state != null)
                return state.EventNumber;

            var newState = new ProjectionStateItem { Name = projectionName, EventNumber = -1 };
            context.Add(newState);
            await context.SaveChangesAsync();
            return newState.EventNumber;
        }

        public async Task UpdateProjectionState(string projectionName, int lastEventNumber)
        {
            await using var context = _contextFactory.Create();
            var state =
                context.ProjectionStates
                    .SingleOrDefault(item => item.Name == projectionName);

            if (state.EventNumber != lastEventNumber)
            {
                state.EventNumber = lastEventNumber;
                state.LastUpdatedUtc = _dateTimeProvider.UtcNow;
            }
            await context.SaveChangesAsync();
        }
    }
}
