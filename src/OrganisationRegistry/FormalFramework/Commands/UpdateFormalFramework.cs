namespace OrganisationRegistry.FormalFramework.Commands;

using FormalFrameworkCategory;

public class UpdateFormalFramework : BaseCommand<FormalFrameworkId>
{
    public FormalFrameworkId FormalFrameworkId => Id;

    public string Name { get; }
    public string Code { get; }
    public FormalFrameworkCategoryId FormalFrameworkCategoryId { get; }

    public UpdateFormalFramework(
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
