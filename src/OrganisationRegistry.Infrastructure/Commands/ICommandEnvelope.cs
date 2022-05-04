namespace OrganisationRegistry.Infrastructure.Commands;

using Authorization;

public record CommandEnvelope<TCommand>(TCommand Command, IUser User) : ICommandEnvelope<TCommand>;

public interface ICommandEnvelope
{
}

public interface ICommandEnvelope<out TCommand> : ICommandEnvelope
{
    public TCommand Command { get; }
    public IUser User { get; }
}
