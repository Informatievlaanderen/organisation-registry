namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;

public class WorkRulesUrl : StringValueObject<WorkRulesUrl>
{
    private static readonly Regex Regex = new("^(.+\\.[pP][dD][fF])?$", RegexOptions.Compiled);

    public WorkRulesUrl(string? workRulesUrl) : base(workRulesUrl ?? "")
    {
        workRulesUrl ??= "";

        if (!EndsWithPdf(workRulesUrl))
            throw new WorkRuleUrlShouldBePdf();

        if (!IsValidUrl(workRulesUrl))
            throw new WorkRuleUrlShouldBeValidUrl();
    }

    public static IEnumerable<string> IsValid(string? workRulesUrl)
    {
        workRulesUrl ??= "";

        if (!EndsWithPdf(workRulesUrl))
            yield return new WorkRuleUrlShouldBePdf().Message;

        if (!IsValidUrl(workRulesUrl))
            yield return new WorkRuleUrlShouldBeValidUrl().Message;
    }

    private static bool EndsWithPdf(string workRulesUrl)
        => Regex.IsMatch(workRulesUrl);

    private static bool IsValidUrl(string workRulesUrl)
        => string.IsNullOrWhiteSpace(workRulesUrl) || Uri.TryCreate(workRulesUrl, UriKind.Absolute, out _);
}
