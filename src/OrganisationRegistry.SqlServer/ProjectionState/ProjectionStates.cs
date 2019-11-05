namespace OrganisationRegistry.SqlServer.ProjectionState
{
    using System;
    using System.Linq;
    using Autofac.Features.OwnedInstances;
    using Infrastructure;

    public class ProjectionStates : IProjectionStates
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;

        public ProjectionStates(Func<Owned<OrganisationRegistryContext>> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public int GetLastProcessedEventNumber(string projectionName)
        {
            using (var context = _contextFactory().Value)
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
            using (var context = _contextFactory().Value)
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
