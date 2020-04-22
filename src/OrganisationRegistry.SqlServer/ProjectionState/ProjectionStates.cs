namespace OrganisationRegistry.SqlServer.ProjectionState
{
    using System;
    using System.Linq;
    using Autofac.Features.OwnedInstances;
    using Infrastructure;

    public class ProjectionStates : IProjectionStates
    {
        private readonly IContextFactory _contextFactory;
        public ProjectionStates(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public int GetLastProcessedEventNumber(string projectionName)
        {
            using (var context = _contextFactory.Create())
            {
                var state =
                    context.ProjectionStates
                        .SingleOrDefault(item => item.Name == projectionName);

                if (state != null)
                    return state.EventNumber;

                var newState = new ProjectionStateItem { Name = projectionName, EventNumber = -1 };
                context.Add(newState);
                context.SaveChanges();
                return newState.EventNumber;
            }
        }

        public void UpdateProjectionState(string projectionName, int lastEventNumber)
        {
            using (var context = _contextFactory.Create())
            {
                var state =
                    context.ProjectionStates
                        .SingleOrDefault(item => item.Name == projectionName);

                state.EventNumber = lastEventNumber;
                context.SaveChanges();
            }
        }
    }
}
