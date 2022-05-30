namespace OrganisationRegistry.FormalFrameworkCategory.Commands;

public class UpdateFormalFrameworkCategory : BaseCommand<FormalFrameworkCategoryId>
{
    public FormalFrameworkCategoryId FormalFrameworkCategoryId => Id;

    public string Name { get; }

    public UpdateFormalFrameworkCategory(
        FormalFrameworkCategoryId formalFrameworkCategoryId,
        string name)
    {
        Id = formalFrameworkCategoryId;
        Name = name;
    }
}