namespace OrganisationRegistry.Tests.Shared;

using System;

public static class TypeExtensions
{
    public static string GetAssociatedResourceDat(this Type type)
        => type.GetResourceString(extension: "dat");

    public static string GetAssociatedResourceCsv(this Type type)
        => type.GetResourceString(extension: "csv");

    public static string GetAssociatedResourceXml(this Type type)
        => type.GetResourceString(extension: "xml");

    /// <summary>
    /// usage: put a csv-file next to the type
    /// it should have the same name as the type, but with an added suffix
    /// like this: typename_suffix.csv (mind the underscore)
    /// then you pass "suffix" into the filenameSuffix parameter of this method
    /// </summary>
    /// <returns>the contents of the embedded csv that matches the calculated filename</returns>
    public static string GetAssociatedResourceCsv(this Type type, string filenameSuffix)
        => type.GetResourceString(suffix: filenameSuffix, extension: "csv");

    private static string GetResourceString(this Type type, string? suffix = null, string? extension = null)
    {
        var resourceName = type.FullName!;

        if (!string.IsNullOrWhiteSpace(suffix))
            resourceName = $"{resourceName}_{suffix}";

        if (!string.IsNullOrWhiteSpace(extension))
            resourceName = $"{resourceName}.{extension}";

        return type.Assembly.GetResourceString(resourceName);
    }
}
