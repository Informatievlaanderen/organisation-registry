namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFrameworkParents;

using System;
using System.Collections.Generic;
using FormalFramework;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using Xunit.Abstractions;

public class WhenCurrentParentIsStillActive : OldSpecification2<UpdateOrganisationFormalFrameworkParentsCommandHandler, UpdateOrganisationFormalFrameworkParents>
{
    private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new();
    private readonly DateTimeProviderStub _dateTimeProvider = new(DateTime.Now);

    private Guid _organisationCreatedId;
    private Guid _formalFrameworkCreatedId;

    public WhenCurrentParentIsStillActive(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override IUser User
        => new UserBuilder()
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        var organisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        _organisationCreatedId = organisationCreated.Id;

        var parentOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedBuilder();

        var formalFrameworkCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
        _formalFrameworkCreatedId = formalFrameworkCreated.Id;

        var organisationFormalFrameworkAdded =
            new OrganisationFormalFrameworkAddedBuilder(
                    organisationCreated.Id,
                    formalFrameworkCreated.Id,
                    parentOrganisationCreated.Id)
                .WithValidity(_dateTimeProvider.Today, _dateTimeProvider.Today.AddDays(1));

        var formalFrameworkAssignedToOrganisation =
            new FormalFrameworkAssignedToOrganisationBuilder(
                organisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
                organisationFormalFrameworkAdded.FormalFrameworkId,
                organisationFormalFrameworkAdded.OrganisationId,
                organisationFormalFrameworkAdded.ParentOrganisationId);

        return new List<IEvent>
        {
            organisationCreated.Build(),
            parentOrganisationCreated.Build(),
            formalFrameworkCategoryCreated.Build(),
            formalFrameworkCreated.Build(),
            organisationFormalFrameworkAdded.Build(),
            formalFrameworkAssignedToOrganisation.Build()
        };
    }

    protected override UpdateOrganisationFormalFrameworkParents When()
        => new(new OrganisationId(_organisationCreatedId), new FormalFrameworkId(_formalFrameworkCreatedId));

    protected override UpdateOrganisationFormalFrameworkParentsCommandHandler BuildHandler()
        => new(
            Mock.Of<ILogger<UpdateOrganisationFormalFrameworkParentsCommandHandler>>(),
            Session,
            _dateTimeProvider);

    protected override int ExpectedNumberOfEvents
        => 0;
}
