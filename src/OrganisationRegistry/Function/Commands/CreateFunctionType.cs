namespace OrganisationRegistry.Function.Commands;

public class CreateFunctionType : BaseCommand<FunctionTypeId>
{
    public FunctionTypeId FunctionTypeId => Id;

    public string Name { get; }

    public CreateFunctionType(
        FunctionTypeId functionTypeId,
        string name)
    {
        Id = functionTypeId;
        Name = name;
    }
}
