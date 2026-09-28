namespace OrganisationRegistry.ArchitectureTests;

using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using Xunit;

/// <summary>Waarom: gedeelde ArchUnitNET-architectuur die één keer de relevante assemblies laadt.</summary>
public abstract class ArchitectureTestBase
{
    private static readonly Architecture ArchitectureInstance = new ArchLoader()
        .LoadAssemblies(
            typeof(Api.Infrastructure.OrganisationRegistryController).Assembly,
            typeof(Infrastructure.Authorization.IUser).Assembly,
            typeof(Organisation.Organisation).Assembly)
        .Build();

    protected static Architecture Architecture => ArchitectureInstance;
}
