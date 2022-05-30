namespace OrganisationRegistry.KboMutations.Ftps;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class FtpsListParser
{
    private static readonly Regex FtpListRegex = new Regex(
        @"^([d-])([rwxt-]{3}){3}\s+\d{1,}\s+.*?(?<size>\d{1,})\s+(?<date>\w+\s+\d{1,2}\s+(?:\d{4})?)(?<time>\d{1,2}:\d{2})?\s+(?<name>.+?)\s?$",
        RegexOptions.Compiled |
        RegexOptions.Multiline |
        RegexOptions.IgnoreCase |
        RegexOptions.IgnorePatternWhitespace);

    public static IEnumerable<FtpsListItem> Parse(FtpUriBuilder sourcePath, string result)
    {
        var matchCollection = FtpListRegex.Matches(result);
        return matchCollection
            .Select(match =>
            {
                var name = match.Groups["name"].Value;
                return new FtpsListItem(
                    name,
                    sourcePath.AppendFileName(name).Path,
                    sourcePath.Path,
                    match.Groups["size"].Value);
            })
            .ToList();
    }
}