namespace OrganisationRegistry.Infrastructure.Commands
{
    using Messages;

    public interface ICommandHandler<in T> : IHandler<T> where T : ICommand
    {
        void Handle(T message);
    }
}
