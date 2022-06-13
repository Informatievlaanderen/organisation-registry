namespace OrganisationRegistry.Tests.Shared.TestDataBuilders;

using System;
using System.Collections.Generic;
using Organisation;
using Organisation.Events;

public class OrganisationCreatedBuilder
{
    public OrganisationId Id { get; private set; }
    public string Name { get; private set; }
    public string OvoNumber { get; private set; }
    public string? ShortName { get; private set; }
    public string? Description { get; }
    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public DateTime? OperationalValidFrom { get; private set; }
    public DateTime? OperationalValidTo { get; private set; }
    public OrganisationSource? SourceType { get; private set; }
    public Guid? SourceId { get; private set; }
    public string? SourceOrganisationIdentifier { get; private set; }
    public Article? Article { get; set; }

    public OrganisationCreatedBuilder(IOvoNumberGenerator ovoNumberGenerator)
    {
        Id = new OrganisationId(Guid.NewGuid());
        Name = Id.ToString();
        OvoNumber = ovoNumberGenerator.GenerateNumber();
        ShortName = default;
        Description = default;
        ValidFrom = null;
        ValidTo = null;
        OperationalValidFrom = null;
        OperationalValidTo = null;
    }

    public OrganisationCreatedBuilder WithId(OrganisationId organisationId)
    {
        Id = organisationId;
        return this;
    }

    public OrganisationCreatedBuilder WithOvoNumber(string ovoNumber)
    {
        OvoNumber = ovoNumber;
        return this;
    }

    public OrganisationCreatedBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    public OrganisationCreatedBuilder WithShortName(string? shortName)
    {
        ShortName = shortName;
        return this;
    }

    public OrganisationCreatedBuilder WithSource(
        OrganisationSource sourceType,
        Guid sourceId,
        string sourceOrganisationIdentifier)
    {
        SourceType = sourceType;
        SourceId = sourceId;
        SourceOrganisationIdentifier = sourceOrganisationIdentifier;
        return this;
    }

    public OrganisationCreatedBuilder WithValidity(DateTime? from, DateTime? to)
    {
        ValidFrom = from;
        ValidTo = to;
        return this;
    }

    public OrganisationCreatedBuilder WithOperationalValidity(DateTime? from, DateTime? to)
    {
        OperationalValidFrom = from;
        OperationalValidTo = to;
        return this;
    }

    public OrganisationCreatedBuilder WithArticle(Article? article)
    {
        Article = article;
        return this;
    }

    public OrganisationCreated Build()
        => new(
            Id,
            Name,
            OvoNumber,
            ShortName,
            Article ?? Article.None,
            Description,
            new List<Purpose>(),
            false,
            ValidFrom,
            ValidTo,
            OperationalValidFrom,
            OperationalValidTo,
            SourceType,
            SourceId,
            SourceOrganisationIdentifier);

    public static implicit operator OrganisationCreated(OrganisationCreatedBuilder builder)
        => builder.Build();
}
