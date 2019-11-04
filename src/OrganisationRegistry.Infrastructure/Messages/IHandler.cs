namespace OrganisationRegistry.Infrastructure.Messages
{
    public interface IHandler<in T> where T : IMessage
    {
    }
}
