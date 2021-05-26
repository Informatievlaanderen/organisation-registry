namespace OrganisationRegistry.SqlServer.ProjectionState
{
    using System.Threading.Tasks;

    public interface IProjectionStates
    {
        Task<int> GetLastProcessedEventNumber(string projectionName);

        Task UpdateProjectionState(string projectionName, int lastEventNumber);
    }
}
