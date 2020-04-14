namespace OrganisationRegistry.Infrastructure.Commands
{
    using System.Threading.Tasks;

    public interface ICommandSender
    {
        Task Send<T>(T command) where T : ICommand;
    }
}
