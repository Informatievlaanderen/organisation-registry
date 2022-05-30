namespace OrganisationRegistry.Infrastructure.Commands;

using Messages;

public interface ICommand : IMessage
{
    /// <summary>
    /// The Expected Version which the Aggregate will become.
    /// </summary>
    int ExpectedVersion { get; set; }
}