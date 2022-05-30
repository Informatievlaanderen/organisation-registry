namespace OrganisationRegistry.Function.Commands;

public class UpdateFunctionType : BaseCommand<FunctionTypeId>
{
    public FunctionTypeId FunctionTypeId => Id;

    public string Name { get; }

    public UpdateFunctionType(
        FunctionTypeId functionTypeId,
        string name)
    {
        Id = functionTypeId;
        Name = name;
    }
}