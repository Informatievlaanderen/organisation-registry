namespace OrganisationRegistry.Infrastructure.Infrastructure;

using System;
using System.Globalization;

public static class ToTitleCaseExtension
{
    public static string ToTitleCase(this string value)
    {
        var cultureInfo = CultureInfo.CurrentCulture;
        return cultureInfo.TextInfo.ToTitleCase(value.ToLower());
    }

    public static string ToTitleCase(this TextInfo textInfo, string str)
    {
        var tokens = str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < tokens.Length; i++)
        {
            var token = tokens[i];
            tokens[i] = token.Substring(0, 1).ToUpper() + token.Substring(1);
        }

        return string.Join(" ", tokens);
    }
}