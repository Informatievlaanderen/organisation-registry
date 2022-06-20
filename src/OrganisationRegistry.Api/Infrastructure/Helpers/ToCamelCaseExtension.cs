namespace OrganisationRegistry.Api.Infrastructure.Helpers;

public static class ToCamelCaseExtension
{
    public static string ToCamelCase(this string s)
        => string.IsNullOrWhiteSpace(s) ? s : s.Insert(0, s[0].ToString().ToLower()).Remove(1, 1);
}
