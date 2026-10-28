namespace OrganisationRegistry.Function.Events;

using System;

public class FunctionTypeRemoved : BaseEvent<FunctionTypeRemoved>
{
    public Guid FunctionTypeId => Id;

    public FunctionTypeRemoved(Guid functionTypeId)
        => Id = functionTypeId;
}
