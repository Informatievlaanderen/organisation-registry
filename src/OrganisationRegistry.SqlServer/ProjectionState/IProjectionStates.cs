namespace OrganisationRegistry.SqlServer.ProjectionState
{
    public interface IProjectionStates
    {
        int GetLastProcessedEventNumber(string projectionName);

        void UpdateProjectionState(string projectionName, int lastEventNumber);
    }
}
