namespace OrganisationRegistry.Infrastructure.Messages;

using System;

public interface IMessage
{
    /// <summary>
    /// The ID of the Aggregate being affected by this event
    /// </summary>
    Guid Id { get; set; }
}