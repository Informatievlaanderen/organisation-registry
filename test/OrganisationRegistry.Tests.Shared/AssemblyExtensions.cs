namespace OrganisationRegistry.Tests.Shared;

using System;
using System.IO;
using System.Reflection;
using System.Text;

public static class AssemblyExtensions
{
    public static string GetResourceString(this Assembly assembly, string resourceName, Encoding? encoding = null)
    {
        var stream = GetResource(assembly, resourceName);

        encoding ??= Encoding.UTF8;

        using var reader = new StreamReader(stream, encoding);
        return reader.ReadToEnd();
    }

    public static Stream GetResource(this Assembly assembly, string resourceName)
    {
        var maybeStream = assembly.GetManifestResourceStream(resourceName);

        if (maybeStream is not { } stream)
            throw CreateException(assembly, resourceName);

        return stream;
    }

    private static FileNotFoundException CreateException(Assembly assembly, string resourceName)
    {
        var manifestResourceNames = assembly.GetManifestResourceNames();
        var manifestResourceNamesList = string.Join(Environment.NewLine, manifestResourceNames);
        return new FileNotFoundException(
            $"Embedded resource {resourceName} not found in assembly {assembly.FullName}, " +
            $"these are valid resources:{Environment.NewLine}{manifestResourceNamesList}");
    }
}
