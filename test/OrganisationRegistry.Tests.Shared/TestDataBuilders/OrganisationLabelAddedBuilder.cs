namespace OrganisationRegistry.Tests.Shared.TestDataBuilders;

using System;
using AutoFixture;
using Organisation.Events;

public class OrganisationLabelAddedBuilder
{
    private Guid _organisationId;
    private Guid _organisationLabelId;
    private Guid _labelTypeId;
    private string _labelTypeName;
    private string _value;
    private DateTime? _validFrom;
    private DateTime? _validTo;

    public OrganisationLabelAddedBuilder()
    {
        var fixture = new Fixture();
        _organisationId = fixture.Create<Guid>();
        _organisationLabelId = fixture.Create<Guid>();
        _labelTypeId = fixture.Create<Guid>();
        _labelTypeName = fixture.Create<string>();
        _value = fixture.Create<string>();
        _validFrom = default;
        _validTo = default;
    }

    public OrganisationLabelAddedBuilder WithOrganisationId(Guid value)
    {
        _organisationId = value;
        return this;
    }

    public OrganisationLabelAddedBuilder WithOrganisationLabelId(Guid value)
    {
        _organisationLabelId = value;
        return this;
    }

    public OrganisationLabelAddedBuilder WithLabelTypeId(Guid value)
    {
        _labelTypeId = value;
        return this;
    }

    public OrganisationLabelAddedBuilder WithLabelTypeName(string value)
    {
        _labelTypeName = value;
        return this;
    }

    public OrganisationLabelAddedBuilder WithValue(string value)
    {
        _value = value;
        return this;
    }

    public OrganisationLabelAddedBuilder WithoutValidFrom()
    {
        _validFrom = default;
        return this;
    }

    public OrganisationLabelAddedBuilder WithValidFrom(DateTime value)
    {
        _validFrom = value;
        return this;
    }


    public OrganisationLabelAddedBuilder WithoutValidTo()
    {
        _validTo = default;
        return this;
    }

    public OrganisationLabelAddedBuilder WithValidTo(DateTime value)
    {
        _validTo = value;
        return this;
    }

    public OrganisationLabelAdded Build()
        => new(
            _organisationId,
            _organisationLabelId,
            _labelTypeId,
            _labelTypeName,
            _value,
            _validFrom,
            _validTo
        );

    public static implicit operator OrganisationLabelAdded(OrganisationLabelAddedBuilder builder)
        => builder.Build();
}
