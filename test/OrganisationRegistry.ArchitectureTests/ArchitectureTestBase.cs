namespace OrganisationRegistry.ArchitectureTests;

using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;

/// <summary>
/// Shared architecture context: loads the Api, domain and infrastructure assemblies once.
/// ArchUnitNET reads compiled binaries — run in Debug configuration.
/// </summary>
public abstract class ArchitectureTestBase
{
    protected static readonly System.Reflection.Assembly ApiAssembly = typeof(Api.Infrastructure.OrganisationRegistryController).Assembly;
    protected static readonly System.Reflection.Assembly DomainAssembly = typeof(Handling.Authorization.ISecurityPolicy).Assembly;
    protected static readonly System.Reflection.Assembly InfrastructureAssembly = typeof(Infrastructure.Authorization.Permission).Assembly;

    protected static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(ApiAssembly, DomainAssembly, InfrastructureAssembly)
        .Build();
}
