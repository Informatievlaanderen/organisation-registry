namespace OrganisationRegistry.Infrastructure.Messages;

// ReSharper disable once UnusedTypeParameter
public interface IHandler<in T> where T : IMessage { }
