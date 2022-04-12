namespace OrganisationRegistry.Tests.Shared.TestDataBuilders;

using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Kernel;
using Organisation;
using Purpose;

public class OrganisationBuilder
{
    private OrganisationId _organisationId;
    private string _name;
    private string _ovoNumber;
    private string _shortName;
    private Article _article;
    private Organisation? _parentOrganisation;
    private string _decription;
    private IEnumerable<Purpose> _purposes;
    private bool _showOnVlaamseOverheidSites;
    private Period _validity;
    private Period _operationalValidity;
    private readonly IDateTimeProvider _dateTimeProvider;

    public OrganisationBuilder(ISpecimenBuilder fixture, IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;

        _organisationId = fixture.Create<OrganisationId>();
        _name = fixture.Create<string>();
        _ovoNumber = fixture.Create<string>();
        _shortName = fixture.Create<string>();
        _parentOrganisation = null;
        _decription = fixture.Create<string>();
        _article = Article.None;
        _purposes = new List<Purpose>();
        _showOnVlaamseOverheidSites = fixture.Create<bool>();
        _validity = fixture.Create<Period>();
        _operationalValidity = fixture.Create<Period>();
    }

    public OrganisationBuilder WithId(Guid value)
    {
        _organisationId = new OrganisationId(value);
        return this;
    }

    public OrganisationBuilder WithName(string value)
    {
        _name = value;
        return this;
    }

    public OrganisationBuilder WithOvoNumber(string value)
    {
        _ovoNumber = value;
        return this;
    }

    public OrganisationBuilder WithShortName(string value)
    {
        _shortName = value;
        return this;
    }

    public OrganisationBuilder WithArticle(Article value)
    {
        _article = value;
        return this;
    }

    public OrganisationBuilder WithParentOrganisation(Organisation value)
    {
        _parentOrganisation = value;
        return this;
    }

    public OrganisationBuilder WithDescription(string value)
    {
        _decription = value;
        return this;
    }

    public OrganisationBuilder WithPurposes(IEnumerable<Purpose> value)
    {
        _purposes = value;
        return this;
    }

    public OrganisationBuilder WithShowOnVlaamseOverheidSites(bool value)
    {
        _showOnVlaamseOverheidSites = value;
        return this;
    }

    public OrganisationBuilder WithValidity(Period value)
    {
        _validity = value;
        return this;
    }

    public OrganisationBuilder WithOperationalValidity(Period value)
    {
        _operationalValidity = value;
        return this;
    }

    public Organisation Build()
        => Organisation.Create(
            _organisationId,
            _name,
            _ovoNumber,
            _shortName,
            _article,
            _parentOrganisation,
            _decription,
            _purposes,
            _showOnVlaamseOverheidSites,
            _validity,
            _operationalValidity,
            _dateTimeProvider);

    public static implicit operator Organisation(OrganisationBuilder builder)
        => builder.Build();
}
