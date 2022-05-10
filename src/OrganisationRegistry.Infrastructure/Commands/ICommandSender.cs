namespace OrganisationRegistry.Infrastructure.Commands
{
    using System.Threading.Tasks;
    using Authorization;

    public interface ICommandSender
    {
        Task Send<T>(T command, IUser? user = null) where T : ICommand;
    }
}
