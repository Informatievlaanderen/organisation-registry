namespace OrganisationRegistry.Api.Backoffice.Parameters.KeyType.Requests;

using System;
using KeyTypes;
using KeyTypes.Commands;

public class RemoveKeyTypeRequest
{
    public Guid KeyTypeId { get; set; }

    public RemoveKeyTypeRequest(Guid keyTypeId)
    {
        KeyTypeId = keyTypeId;
    }
}

public static class RemoveKeyTypeRequestMapping
{
    public static RemoveKeyType Map(RemoveKeyTypeRequest message)
        => new(new KeyTypeId(message.KeyTypeId));
}
