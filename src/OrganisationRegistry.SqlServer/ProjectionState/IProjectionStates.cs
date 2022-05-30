namespace OrganisationRegistry.SqlServer.ProjectionState;

using System.Data.Common;
using System.Threading.Tasks;

public interface IProjectionStates
{
    Task<int> GetLastProcessedEventNumber(string projectionName);

    Task UpdateProjectionState(string projectionName, int lastEventNumber, DbConnection? connection = null,
        DbTransaction? transaction = null);
    Task<bool> Exists(string projectionName);
    Task Remove(string projectionName);
}