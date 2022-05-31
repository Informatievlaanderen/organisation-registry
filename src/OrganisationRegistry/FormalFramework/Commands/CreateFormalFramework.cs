namespace OrganisationRegistry.FormalFramework.Commands;

using FormalFrameworkCategory;

public class CreateFormalFramework : BaseCommand<FormalFrameworkId>
{
    public FormalFrameworkId FormalFrameworkId => Id;

    public string Name { get; }
    public string Code { get; }
    public FormalFrameworkCategoryId FormalFrameworkCategoryId { get; }

    public CreateFormalFramework(
        FormalFrameworkId formalFrameworkId,
        string name,
        string code,
        FormalFrameworkCategoryId formalFrameworkCategoryId)
    {
        Id = formalFrameworkId;
        Name = name;
        Code = code;
        FormalFrameworkCategoryId = formalFrameworkCategoryId;
    }
}
