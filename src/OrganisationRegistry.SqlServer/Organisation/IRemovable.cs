namespace OrganisationRegistry.SqlServer.Organisation;

public interface IRemovable
{
    public bool ScheduledForRemoval { get; }
}
