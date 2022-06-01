namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validators;

using System.Collections.Generic;
using System.Linq;

public static class InvalidArticle
{
    public static ValidationIssue? Validate(int rowNumber, DeserializedRecord record)
        => ValidationIssuesFactory.Create(rowNumber, CheckInvalidArticle(record).ToList(), FormatMessage);

    private static IEnumerable<string> CheckInvalidArticle(DeserializedRecord record)
    {
        if (!record.Article.ShouldHaveValue)
            yield break;

        var article = record.Article.Value ?? "";
        var validArticles = new[] { "de", "het", "" };
        if (!validArticles.Contains(article))
            yield return article;
    }

    public static string FormatMessage(string article)
        => $"De waarde {article} is ongeldig voor column 'article' (Geldige waarden: 'de', 'het').";
}
