namespace OrganisationRegistry.SqlServer.ProjectionState;

using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            await context.ProjectionStates
                .SingleOrDefaultAsync(item => item.Name == projectionName);

        if (state != null)
            return state.EventNumber;

        var newState = new ProjectionStateItem { Name = projectionName, EventNumber = -1 };
        context.Add(newState);
        await context.SaveChangesAsync();
        return newState.EventNumber;
    }

    public async Task UpdateProjectionState(string projectionName, int lastEventNumber, DbConnection? connection = null, DbTransaction? transaction = null)
    {
        await using var context = connection != null && transaction != null ?
            _contextFactory.CreateTransactional(connection, transaction) :
            _contextFactory.Create();
        var state = await context.ProjectionStates
            .SingleAsync(item => item.Name == projectionName);

        if (state.EventNumber != lastEventNumber)
        {
            state.EventNumber = lastEventNumber;
            state.LastUpdatedUtc = _dateTimeProvider.UtcNow;
        }
        await context.SaveChangesAsync();
    }

    public async Task<bool> Exists(string projectionName)
    {
        await using var context = _contextFactory.Create();
        return await context.ProjectionStates.AnyAsync(x => x.Name == projectionName);
    }

    public async Task Remove(string projectionName)
    {
        await using var context = _contextFactory.Create();
        var projectionStateItem = await context.ProjectionStates.SingleOrDefaultAsync(x => x.Name == projectionName);

        if (projectionStateItem == null)
            return;

        context.ProjectionStates.Remove(projectionStateItem);
    }
}
