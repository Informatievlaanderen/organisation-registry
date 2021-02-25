namespace OrganisationRegistry.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using AutoFixture.Kernel;
    using FluentAssertions;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.OrganisationTermination;
    using OrganisationRegistry.Organisation.State;
    using Xunit;

    public class OrganisationTerminationTests
    {
        private static List<T> CreateFieldsInThePastOf<T>(DateTime dateOfTermination, ISpecimenBuilder fixture) where T: IOrganisationField, IValidityBuilder<T>
        {
            return fixture.CreateMany<T>()
                .Select(x => x.WithValidity(
                    new Period(
                        new ValidFrom(),
                        new ValidTo(dateOfTermination.AddDays(fixture.Create<int>() * -1)))))
                .ToList();
        }

        private static List<T> CreateFieldsOverlappingWith<T>(DateTime dateOfTermination, ISpecimenBuilder fixture) where T: IOrganisationField, IValidityBuilder<T>
        {
            return fixture.CreateMany<T>()
                .Select(x => x.WithValidity(
                    new Period(
                        new ValidFrom(dateOfTermination.AddDays(fixture.Create<int>() * -1)),
                        new ValidTo(dateOfTermination.AddDays(fixture.Create<int>() * +1)))))
                .ToList();
        }

        [Fact]
        public void ThrowsWhenOrganisationValidityIsInFutureOfTerminationDate()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationState = new OrganisationState
            {
                Validity = new Period(
                    new ValidFrom(dateOfTermination.AddDays(1)),
                    new ValidTo(dateOfTermination.AddYears(1)))
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    organisationState));
        }

        [Fact]
        public void DoesNotOverwriteNewValidToWhenOnSameDayAsDateOfTermination()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationState = new OrganisationState
            {
                Validity = new Period(
                    new ValidFrom(dateOfTermination),
                    new ValidTo(dateOfTermination))
            };

            OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    organisationState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationSummary
                    {
                        OrganisationNewValidTo = null
                    });
        }

        [Fact]
        public void DoesNotOverwriteNewValidToWhenInFutureOfDateOfTermination()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationState = new OrganisationState
            {
                Validity = new Period(
                    new ValidFrom(dateOfTermination.AddDays(-1)),
                    new ValidTo(dateOfTermination.AddDays(-1)))
            };

            OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    organisationState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationSummary
                    {
                        OrganisationNewValidTo = null
                    });
        }

        [Fact]
        public void OverwritesNewValidToWhenValiditySpansOverDateOfTermination()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationState = new OrganisationState
            {
                Validity = new Period(
                    new ValidFrom(dateOfTermination.AddDays(-1)),
                    new ValidTo(dateOfTermination.AddDays(1)))
            };

            OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    organisationState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationSummary
                    {
                        OrganisationNewValidTo = dateOfTermination
                    });
        }

        [Fact]
        public void ThrowsWhenAnyContactValidityIsInFutureOfTerminationDate()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationState = new OrganisationState
            {
                OrganisationContacts =
                {
                    fixture.Create<OrganisationContact>()
                        .WithValidity(new Period(
                            new ValidFrom(dateOfTermination.AddDays(1)),
                            new ValidTo()))
                }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    organisationState));
        }

        [Fact]
        public void ThrowsWhenAnyFormalFrameworkValidityIsInFutureOfTerminationDate()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationState = new OrganisationState
            {
                OrganisationFormalFrameworks =
                {
                    fixture.Create<OrganisationFormalFramework>()
                        .WithValidity(new Period(
                            new ValidFrom(dateOfTermination.AddDays(1)),
                            new ValidTo()))
                }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    organisationState));
        }

        [Fact]
        public void ThrowsWhenAnyClassificationValidityIsInFutureOfTerminationDate()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationState = new OrganisationState
            {
                OrganisationOrganisationClassifications =
                {
                    fixture.Create<OrganisationOrganisationClassification>()
                        .WithValidity(new Period(
                            new ValidFrom(dateOfTermination.AddDays(1)),
                            new ValidTo()))
                }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    organisationState));
        }

        [Fact]
        public void ThrowsWhenAnyCapacityValidityIsInFutureOfTerminationDate()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationState = new OrganisationState
            {
                OrganisationCapacities =
                {
                    fixture.Create<OrganisationCapacity>()
                        .WithValidity(new Period(
                            new ValidFrom(dateOfTermination.AddDays(1)),
                            new ValidTo()))
                }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    organisationState));
        }

        [Fact]
        public void ThrowsWhenAnyFormalFrameworkToTerminateEndOfNextYearIsInFutureOfEndOfNextYear()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationFormalFramework = fixture.Create<OrganisationFormalFramework>()
                .WithValidity(new Period(
                    new ValidFrom(dateOfTermination.AddYears(2)),
                    new ValidTo()));
            var organisationState = new OrganisationState
            {
                OrganisationFormalFrameworks =
                {
                    organisationFormalFramework
                }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    new[] {organisationFormalFramework.FormalFrameworkId},
                    organisationState));
        }


        [Fact]
        public void ThrowsWhenAnyClassificationToTerminateEndOfNextYearIsInFutureOfEndOfNextYear()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationClassification = fixture.Create<OrganisationOrganisationClassification>()
                .WithValidity(new Period(
                    new ValidFrom(dateOfTermination.AddYears(2)),
                    new ValidTo()));
            var organisationState = new OrganisationState
            {
                OrganisationOrganisationClassifications =
                {
                    organisationClassification
                }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    new[] {organisationClassification.OrganisationClassificationTypeId},
                    Enumerable.Empty<Guid>(),
                    organisationState));
        }

        [Fact]
        public void ThrowsWhenAnyCapacityToTerminateEndOfNextYearIsInFutureOfEndOfNextYear()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationCapacity = fixture.Create<OrganisationCapacity>()
                .WithValidity(new Period(
                    new ValidFrom(dateOfTermination.AddYears(2)),
                    new ValidTo()));

            var organisationState = new OrganisationState
            {
                OrganisationCapacities =
                {
                    organisationCapacity
                }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    new[] {organisationCapacity.OrganisationCapacityId},
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    organisationState));
        }


        [Fact]
        public void ThrowsWhenAnyOpeningHourValidityIsInFutureOfTerminationDate()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationOpeningHour = fixture.Create<OrganisationOpeningHour>()
                .WithValidity(new Period(
                    new ValidFrom(dateOfTermination.AddDays(1)),
                    new ValidTo()));

            var organisationState = new OrganisationState
            {
                OrganisationOpeningHours =
                {
                    organisationOpeningHour
                }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    organisationState));
        }

        [Fact]
        public void OverwritesFieldsWhenValiditySpansOverDateOfTermination()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var overlappingContacts = CreateFieldsOverlappingWith<OrganisationContact>(dateOfTermination, fixture);
            var overlappingOpeningHours = CreateFieldsOverlappingWith<OrganisationOpeningHour>(dateOfTermination, fixture);
            var overlappingLabels = CreateFieldsOverlappingWith<OrganisationLabel>(dateOfTermination, fixture);
            var overlappingFunctions = CreateFieldsOverlappingWith<OrganisationFunction>(dateOfTermination, fixture);
            var overlappingLocations = CreateFieldsOverlappingWith<OrganisationLocation>(dateOfTermination, fixture);
            var overlappingBuildings = CreateFieldsOverlappingWith<OrganisationBuilding>(dateOfTermination, fixture);
            var overlappingParents = CreateFieldsOverlappingWith<OrganisationParent>(dateOfTermination, fixture);
            var overlappingBankAccounts = CreateFieldsOverlappingWith<OrganisationBankAccount>(dateOfTermination, fixture);
            var overlappingRelations = CreateFieldsOverlappingWith<OrganisationRelation>(dateOfTermination, fixture);
            var overlappingCapacities = CreateFieldsOverlappingWith<OrganisationCapacity>(dateOfTermination, fixture);
            var overlappingClassifications = CreateFieldsOverlappingWith<OrganisationOrganisationClassification>(dateOfTermination, fixture);
            var overlappingFormalFrameworks = CreateFieldsOverlappingWith<OrganisationFormalFramework>(dateOfTermination, fixture);

            var organisationState = new OrganisationState();
            organisationState.OrganisationContacts.AddRange(overlappingContacts);
            organisationState.OrganisationContacts.AddRange(CreateFieldsInThePastOf<OrganisationContact>(dateOfTermination, fixture));
            organisationState.OrganisationOpeningHours.AddRange(overlappingOpeningHours);
            organisationState.OrganisationOpeningHours.AddRange(CreateFieldsInThePastOf<OrganisationOpeningHour>(dateOfTermination, fixture));
            organisationState.OrganisationLabels.AddRange(overlappingLabels);
            organisationState.OrganisationLabels.AddRange(CreateFieldsInThePastOf<OrganisationLabel>(dateOfTermination, fixture));
            organisationState.OrganisationFunctionTypes.AddRange(overlappingFunctions);
            organisationState.OrganisationFunctionTypes.AddRange(CreateFieldsInThePastOf<OrganisationFunction>(dateOfTermination, fixture));
            organisationState.OrganisationLocations.AddRange(overlappingLocations);
            organisationState.OrganisationLocations.AddRange(CreateFieldsInThePastOf<OrganisationLocation>(dateOfTermination, fixture));
            organisationState.OrganisationBuildings.AddRange(overlappingBuildings);
            organisationState.OrganisationBuildings.AddRange(CreateFieldsInThePastOf<OrganisationBuilding>(dateOfTermination, fixture));
            organisationState.OrganisationParents.AddRange(overlappingParents);
            organisationState.OrganisationParents.AddRange(CreateFieldsInThePastOf<OrganisationParent>(dateOfTermination, fixture));
            organisationState.OrganisationBankAccounts.AddRange(overlappingBankAccounts);
            organisationState.OrganisationBankAccounts.AddRange(CreateFieldsInThePastOf<OrganisationBankAccount>(dateOfTermination, fixture));
            organisationState.OrganisationRelations.AddRange(overlappingRelations);
            organisationState.OrganisationRelations.AddRange(CreateFieldsInThePastOf<OrganisationRelation>(dateOfTermination, fixture));
            organisationState.OrganisationCapacities.AddRange(overlappingCapacities);
            organisationState.OrganisationCapacities.AddRange(CreateFieldsInThePastOf<OrganisationCapacity>(dateOfTermination, fixture));
            organisationState.OrganisationOrganisationClassifications.AddRange(overlappingClassifications);
            organisationState.OrganisationOrganisationClassifications.AddRange(CreateFieldsInThePastOf<OrganisationOrganisationClassification>(dateOfTermination, fixture));
            organisationState.OrganisationFormalFrameworks.AddRange(overlappingFormalFrameworks);
            organisationState.OrganisationFormalFrameworks.AddRange(CreateFieldsInThePastOf<OrganisationFormalFramework>(dateOfTermination, fixture));

            OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    organisationState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationSummary
                    {
                        OrganisationNewValidTo = new ValidTo(dateOfTermination),
                        Contacts = overlappingContacts.ToDictionary(x => x.OrganisationContactId, _ => dateOfTermination),
                        OpeningHours = overlappingOpeningHours.ToDictionary(x => x.OrganisationOpeningHourId, _ => dateOfTermination),
                        Labels = overlappingLabels.ToDictionary(x => x.OrganisationLabelId, _ => dateOfTermination),
                        Functions = overlappingFunctions.ToDictionary(x => x.OrganisationFunctionId, _ => dateOfTermination),
                        Locations = overlappingLocations.ToDictionary(x => x.OrganisationLocationId, _ => dateOfTermination),
                        Buildings = overlappingBuildings.ToDictionary(x => x.OrganisationBuildingId, _ => dateOfTermination),
                        Parents = overlappingParents.ToDictionary(x => x.OrganisationOrganisationParentId, _ => dateOfTermination),
                        BankAccounts = overlappingBankAccounts.ToDictionary(x => x.OrganisationBankAccountId, _ => dateOfTermination),
                        Relations = overlappingRelations.ToDictionary(x => x.OrganisationRelationId, _ => dateOfTermination),
                        Capacities = overlappingCapacities.ToDictionary(x => x.OrganisationCapacityId, _ => dateOfTermination),
                        Classifications = overlappingClassifications.ToDictionary(x => x.OrganisationOrganisationClassificationId, _ => dateOfTermination),
                        FormalFrameworks = overlappingFormalFrameworks.ToDictionary(x => x.OrganisationFormalFrameworkId, _ => dateOfTermination),
                    });
        }

        [Fact]
        public void OverwritesFieldsWhenValiditySpansOverEndOfNextYear()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var endOfNextYear = new DateTime(dateOfTermination.Year + 1, 12, 31);

            var capacitiesToTerminateEndOfNextYear = new List<OrganisationCapacity>()
            {
                fixture.Create<OrganisationCapacity>()
                    .WithValidFrom(new ValidFrom(dateOfTermination.AddDays(1)))
                    .WithValidTo(new ValidTo())
            };

            var classificationsToTerminateEndOfNextYear = new List<OrganisationOrganisationClassification>()
            {
                fixture.Create<OrganisationOrganisationClassification>()
                    .WithValidFrom(new ValidFrom(dateOfTermination.AddDays(1)))
                    .WithValidTo(new ValidTo())
            };

            var formalFrameworksToTerminateEndOfNextYear = new List<OrganisationFormalFramework>()
            {
                fixture.Create<OrganisationFormalFramework>()
                    .WithValidFrom(new ValidFrom(dateOfTermination.AddDays(1)))
                    .WithValidTo(new ValidTo())
            };

            var organisationState = new OrganisationState();
            organisationState.OrganisationCapacities.AddRange(capacitiesToTerminateEndOfNextYear);
            organisationState.OrganisationCapacities.AddRange(CreateFieldsInThePastOf<OrganisationCapacity>(dateOfTermination, fixture));
            organisationState.OrganisationOrganisationClassifications.AddRange(classificationsToTerminateEndOfNextYear);
            organisationState.OrganisationOrganisationClassifications.AddRange(CreateFieldsInThePastOf<OrganisationOrganisationClassification>(dateOfTermination, fixture));
            organisationState.OrganisationFormalFrameworks.AddRange(formalFrameworksToTerminateEndOfNextYear);
            organisationState.OrganisationFormalFrameworks.AddRange(CreateFieldsInThePastOf<OrganisationFormalFramework>(dateOfTermination, fixture));

            OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    capacitiesToTerminateEndOfNextYear.Select(x => x.CapacityId),
                    classificationsToTerminateEndOfNextYear.Select(x => x.OrganisationClassificationTypeId),
                    formalFrameworksToTerminateEndOfNextYear.Select(x => x.FormalFrameworkId),
                    organisationState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationSummary
                    {
                        OrganisationNewValidTo = new ValidTo(dateOfTermination),
                        Capacities = capacitiesToTerminateEndOfNextYear.ToDictionary(x => x.OrganisationCapacityId, _ => endOfNextYear),
                        Classifications = classificationsToTerminateEndOfNextYear.ToDictionary(x => x.OrganisationOrganisationClassificationId, _ => endOfNextYear),
                        FormalFrameworks = formalFrameworksToTerminateEndOfNextYear.ToDictionary(x => x.OrganisationFormalFrameworkId, _ => endOfNextYear),
                    });
        }

        [Fact]
        public void OverwritesEndOfNextYearFieldsWhenTypeIdsMatch()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var overlappingCapacities = CreateFieldsOverlappingWith<OrganisationCapacity>(dateOfTermination, fixture);
            var overlappingClassifications = CreateFieldsOverlappingWith<OrganisationOrganisationClassification>(dateOfTermination, fixture);
            var overlappingFormalFrameworks = CreateFieldsOverlappingWith<OrganisationFormalFramework>(dateOfTermination, fixture);

            var organisationState = new OrganisationState();
            organisationState.OrganisationCapacities.AddRange(overlappingCapacities);
            organisationState.OrganisationCapacities.AddRange(CreateFieldsInThePastOf<OrganisationCapacity>(dateOfTermination, fixture));
            organisationState.OrganisationOrganisationClassifications.AddRange(overlappingClassifications);
            organisationState.OrganisationOrganisationClassifications.AddRange(CreateFieldsInThePastOf<OrganisationOrganisationClassification>(dateOfTermination, fixture));
            organisationState.OrganisationFormalFrameworks.AddRange(overlappingFormalFrameworks);
            organisationState.OrganisationFormalFrameworks.AddRange(CreateFieldsInThePastOf<OrganisationFormalFramework>(dateOfTermination, fixture));

            var capacityTypeIdsToTerminateEndOfNextYear = fixture.CreateMany<Guid>();
            var classificationTypeIdsToTerminateEndOfNextYear = fixture.CreateMany<Guid>();
            var formalFrameworkIdsToTerminateEndOfNextYear = fixture.CreateMany<Guid>();

            OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                    capacityTypeIdsToTerminateEndOfNextYear,
                    classificationTypeIdsToTerminateEndOfNextYear,
                    formalFrameworkIdsToTerminateEndOfNextYear,
                    organisationState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationSummary
                    {
                        OrganisationNewValidTo = new ValidTo(dateOfTermination),
                        Capacities = overlappingCapacities.ToDictionary(x => x.OrganisationCapacityId, _ => dateOfTermination),
                        Classifications = overlappingClassifications.ToDictionary(x => x.OrganisationOrganisationClassificationId, _ => dateOfTermination),
                        FormalFrameworks = overlappingFormalFrameworks.ToDictionary(x => x.OrganisationFormalFrameworkId, _ => dateOfTermination),
                    });
        }

        [Fact]
        public void OverwritesKboFieldsWhenKboHasTermination()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var kboState = new KboState();
            kboState.KboBankAccounts.AddRange(CreateFieldsInThePastOf<OrganisationBankAccount>(dateOfTermination, fixture));
            kboState.KboRegisteredOffice = fixture.Create<OrganisationLocation>()
                .WithValidity(
                    new Period(
                        new ValidFrom(),
                        new ValidTo(dateOfTermination.AddYears(fixture.Create<int>() * -1))));
            kboState.KboFormalNameLabel = fixture.Create<OrganisationLabel>()
                .WithValidity(
                    new Period(
                        new ValidFrom(),
                        new ValidTo(dateOfTermination.AddYears(fixture.Create<int>() * -1))));
            kboState.KboLegalFormOrganisationClassification = fixture.Create<OrganisationOrganisationClassification>()
                .WithValidity(
                    new Period(
                        new ValidFrom(),
                        new ValidTo(dateOfTermination.AddYears(fixture.Create<int>() * -1))));
            kboState.TerminationInKbo = new KboTermination(
                dateOfTermination.AddDays(fixture.Create<int>() * -1),
                fixture.Create<string>(),
                fixture.Create<string>());

            OrganisationTerminationCalculator.GetKboFieldsToForceTerminate(dateOfTermination,
                    kboState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationKboSummary
                    {
                        KboBankAccounts = kboState.KboBankAccounts.ToDictionary(x => x.OrganisationBankAccountId, _ => dateOfTermination),
                        KboFormalNameLabel = new KeyValuePair<Guid, DateTime>(kboState.KboFormalNameLabel.OrganisationLabelId, dateOfTermination),
                        KboRegisteredOfficeLocation = new KeyValuePair<Guid, DateTime>(kboState.KboRegisteredOffice.OrganisationLocationId, dateOfTermination),
                        KboLegalForm = new KeyValuePair<Guid, DateTime>(kboState.KboLegalFormOrganisationClassification.OrganisationOrganisationClassificationId, dateOfTermination),
                    });
        }

                [Fact]
        public void OverwritesKboFieldsWhenKboDoesntHaveTermination()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var kboState = new KboState();
            kboState.KboBankAccounts.AddRange(CreateFieldsInThePastOf<OrganisationBankAccount>(dateOfTermination, fixture));
            kboState.KboRegisteredOffice = fixture.Create<OrganisationLocation>()
                .WithValidity(
                    new Period(
                        new ValidFrom(),
                        new ValidTo(dateOfTermination.AddYears(fixture.Create<int>() * -1))));
            kboState.KboFormalNameLabel = fixture.Create<OrganisationLabel>()
                .WithValidity(
                    new Period(
                        new ValidFrom(),
                        new ValidTo(dateOfTermination.AddYears(fixture.Create<int>() * -1))));
            kboState.KboLegalFormOrganisationClassification = fixture.Create<OrganisationOrganisationClassification>()
                .WithValidity(
                    new Period(
                        new ValidFrom(),
                        new ValidTo(dateOfTermination.AddYears(fixture.Create<int>() * -1))));
            kboState.TerminationInKbo = null;

            OrganisationTerminationCalculator.GetKboFieldsToForceTerminate(dateOfTermination,
                    kboState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationKboSummary
                    {
                        KboBankAccounts = kboState.KboBankAccounts.ToDictionary(x => x.OrganisationBankAccountId, _ => dateOfTermination),
                        KboFormalNameLabel = new KeyValuePair<Guid, DateTime>(kboState.KboFormalNameLabel.OrganisationLabelId, dateOfTermination),
                        KboRegisteredOfficeLocation = new KeyValuePair<Guid, DateTime>(kboState.KboRegisteredOffice.OrganisationLocationId, dateOfTermination),
                        KboLegalForm = new KeyValuePair<Guid, DateTime>(kboState.KboLegalFormOrganisationClassification.OrganisationOrganisationClassificationId, dateOfTermination),
                    });
        }
    }
}
