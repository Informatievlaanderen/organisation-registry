namespace OrganisationRegistry.Infrastructure.Commands
{
    using System.Threading.Tasks;
    using Messages;

    public interface ICommandHandler<in T> : IHandler<T> where T : ICommand
    {
        Task Handle(T message);
    }
}
