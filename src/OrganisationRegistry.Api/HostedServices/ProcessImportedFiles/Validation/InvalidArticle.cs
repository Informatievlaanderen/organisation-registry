namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validation;

using System.Collections.Generic;
using System.Linq;

public static class InvalidArticle
{
    private static readonly string[] ValidArticles = { "de", "het", "" };

    public static ValidationIssue? Validate(int rowNumber, params Field[] articleFields)
        => ValidationIssuesFactory.Create(rowNumber, CheckInvalidArticle(articleFields).ToList(), FormatMessage);

    private static IEnumerable<string> CheckInvalidArticle(Field[] fields)
        => fields
            .Where(field => field.ShouldHaveValue)
            .Select(field => field.Value ?? "")
            .Where(article => !ValidArticles.Contains(article));

    public static string FormatMessage(string article)
        => $"De waarde {article} is ongeldig voor kolom 'article' (Geldige waarden: 'de', 'het').";
}
