namespace OrganisationRegistry.Function.Commands;

public class RemoveFunctionType : BaseCommand<FunctionTypeId>
{
    public FunctionTypeId FunctionTypeId => Id;

    public RemoveFunctionType(FunctionTypeId functionTypeId)
    {
        Id = functionTypeId;
    }
}
