namespace OrganisationRegistry.Api.Backoffice.Parameters.FunctionType.Requests;

using System;
using OrganisationRegistry.Function;
using OrganisationRegistry.Function.Commands;

public class RemoveFunctionTypeRequest
{
    public Guid FunctionTypeId { get; set; }

    public RemoveFunctionTypeRequest(Guid functionTypeId)
    {
        FunctionTypeId = functionTypeId;
    }
}

public static class RemoveFunctionTypeRequestMapping
{
    public static RemoveFunctionType Map(RemoveFunctionTypeRequest message)
        => new(new FunctionTypeId(message.FunctionTypeId));
}
