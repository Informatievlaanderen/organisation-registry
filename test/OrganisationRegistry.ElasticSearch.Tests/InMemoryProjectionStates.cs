namespace OrganisationRegistry.ElasticSearch.Tests;

using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using SqlServer.ProjectionState;

/// <summary>
/// In-memory projection state, so consecutive runner invocations pick up where the previous one stopped.
/// An unknown projection reports -1, which is what makes a runner initialise its index on the first run.
/// </summary>
public class InMemoryProjectionStates : IProjectionStates
{
    private readonly Dictionary<string, int> _lastProcessedEventNumbers = new();

    public Task<int> GetLastProcessedEventNumber(string projectionName)
        => Task.FromResult(
            _lastProcessedEventNumbers.TryGetValue(projectionName, out var number) ? number : -1);

    public Task UpdateProjectionState(
        string projectionName,
        int lastEventNumber,
        DbConnection? connection = null,
        DbTransaction? transaction = null)
    {
        _lastProcessedEventNumbers[projectionName] = lastEventNumber;

        return Task.CompletedTask;
    }

    public Task<bool> Exists(string projectionName)
        => Task.FromResult(_lastProcessedEventNumbers.ContainsKey(projectionName));

    public Task Remove(string projectionName)
    {
        _lastProcessedEventNumbers.Remove(projectionName);

        return Task.CompletedTask;
    }
}
