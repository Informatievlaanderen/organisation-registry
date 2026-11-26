namespace OrganisationRegistry.SqlServer.Organisation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Normaliseert codes voor het zoeken op de hoofdpagina (OR-3785): OVO-nummers, KBO-nummers en
/// Orafin-sleutelwaarden. Zoeken is een deelmatch (substring) op de genormaliseerde waarde: een
/// gebruiker die <c>4</c> intikt, krijgt alle organisaties met een 4 in een van haar codes.
/// <br />
/// Dezelfde normalisatie wordt toegepast bij het projecteren (<see cref="Normalize" />) en bij
/// het zoeken (<see cref="Candidates" />).
/// </summary>
public static class OrganisationCodeNormalizer
{
    /// <summary>
    /// De vorm waarin een code bewaard en vergeleken wordt: zonder omringende witruimte, in
    /// hoofdletters, en zonder scheidingstekens. Zo matchen <c>0123.456.789</c> en
    /// <c>0123456789</c>, en <c>ovo001234</c> en <c>OVO001234</c>.
    /// </summary>
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = new StringBuilder(value.Length);

        foreach (var character in value)
        {
            if (char.IsLetterOrDigit(character))
                normalized.Append(char.ToUpperInvariant(character));
        }

        return normalized.ToString();
    }

    /// <summary>
    /// De deelwaarden waarvan een opgeslagen code er minstens één moet bevatten. Naast de
    /// genormaliseerde zoekterm is dat het KBO-nummer zonder Belgische landcode, want
    /// <c>BE0123456789</c> moet ook <c>0123456789</c> vinden. (Een voorloopnul bijvoegen bij negen
    /// cijfers is niet meer nodig: <c>123456789</c> zit al in <c>0123456789</c>.)
    /// <br />
    /// Een lege of louter uit scheidingstekens bestaande zoekterm levert geen kandidaten op; de
    /// query behandelt dat als "niet ingevuld".
    /// </summary>
    public static IReadOnlyList<string> Candidates(string? searchValue)
    {
        var normalized = Normalize(searchValue);
        if (normalized.Length == 0)
            return Array.Empty<string>();

        var candidates = new List<string> { normalized };

        if (TryRemoveBelgianCountryPrefix(normalized, out var withoutCountryPrefix))
            candidates.Add(withoutCountryPrefix);

        return candidates;
    }

    private const string BelgianCountryPrefix = "BE";

    private static bool TryRemoveBelgianCountryPrefix(string normalized, out string withoutCountryPrefix)
    {
        withoutCountryPrefix = string.Empty;

        if (!normalized.StartsWith(BelgianCountryPrefix, StringComparison.Ordinal))
            return false;

        var remainder = normalized[BelgianCountryPrefix.Length..];
        if (remainder.Length is not (9 or 10) || !remainder.All(char.IsDigit))
            return false;

        withoutCountryPrefix = remainder;
        return true;
    }

    /// <summary>Of een waarde na normalisatie een doorzoekbare code oplevert.</summary>
    public static bool HasSearchableValue(string? searchValue)
        => Normalize(searchValue).Length > 0;
}
