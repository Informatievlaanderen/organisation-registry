namespace OrganisationRegistry.Api.Infrastructure.Swagger;

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Voegt beschrijvingen toe aan de tag-groepen in de Swagger UI.
/// </summary>
public class TagDescriptionDocumentFilter : IDocumentFilter
{
    private static readonly IReadOnlyDictionary<string, string> TagDescriptions =
        new Dictionary<string, string>
        {
            ["Scherm APIs: Organisaties"] =
                "Endpoints voor het beheren van organisaties en hun eigenschappen " +
                "zoals locaties, contacten, functies, hoedanigheden, sleutels, benamingen, " +
                "toepassingsgebieden, relaties, regelgeving en bankrekeningen.",

            ["Scherm APIs: Organen"] =
                "Endpoints voor het beheren van organen en hun eigenschappen " +
                "zoals zetels, mandaten, levensloopfasen, toepassingsgebieden, " +
                "organisaties en classificaties.",

            ["Scherm APIs: Personen"] =
                "Endpoints voor het beheren van personen en hun functies, " +
                "hoedanigheden en mandaten.",

            ["Scherm APIs: Parameters"] =
                "Endpoints voor het opvragen en beheren van parameterlijsten " +
                "zoals types, categorieën en delegaties.",

            ["Scherm APIs: Administratie"] =
                "Endpoints voor administratief beheer: configuratie, events, " +
                "projecties en KBO-opzoekingen.",

            ["Scherm APIs: Rapporten"] =
                "Endpoints voor het opvragen van rapporten over organen, " +
                "organisaties, toepassingsgebieden en participatie.",
        };

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var (tag, description) in TagDescriptions)
        {
            swaggerDoc.Tags.Add(new OpenApiTag
            {
                Name = tag,
                Description = description,
            });
        }
    }
}
