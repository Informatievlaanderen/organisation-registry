namespace OrganisationRegistry.Infrastructure.Commands
{
    using Authorization;
    using Messages;

    public interface ICommand : IMessage
    {
        /// <summary>
        /// The Expected Version which the Aggregate will become.
        /// </summary>
        int ExpectedVersion { get; set; }
        IUser? User { get; set; }
    }
}
