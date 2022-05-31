namespace OrganisationRegistry.FormalFrameworkCategory.Commands;

public class CreateFormalFrameworkCategory : BaseCommand<FormalFrameworkCategoryId>
{
    public FormalFrameworkCategoryId FormalFrameworkCategoryId => Id;

    public string Name { get; }

    public CreateFormalFrameworkCategory(
        FormalFrameworkCategoryId formalFrameworkCategoryId,
        string name)
    {
        Id = formalFrameworkCategoryId;
        Name = name;
    }
}
