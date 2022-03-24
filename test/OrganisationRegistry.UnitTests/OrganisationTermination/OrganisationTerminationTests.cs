namespace OrganisationRegistry.UnitTests.OrganisationTermination
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using FluentAssertions;
    using KeyTypes;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Exceptions;
    using OrganisationRegistry.Organisation.OrganisationTermination;
    using OrganisationRegistry.Organisation.State;
    using Tests.Shared.TestDataBuilders;
    using Xunit;

    public class OrganisationTerminationTests
    {
        private static readonly Guid VlimpersKeyTypeId = Guid.NewGuid();
        private static readonly Guid RekenhofFormalFrameworkId = Guid.NewGuid();
        private static readonly Guid RekenhofCapacityId = Guid.NewGuid();
        private static readonly Guid RekenhofClassificationTypeId = Guid.NewGuid();

        private static readonly KeyType VlimpersKeyType = new(new KeyTypeId(VlimpersKeyTypeId), new KeyTypeName("VlimpersKey"));
        private static readonly KeyType OtherKeyType = new(new KeyTypeId(Guid.NewGuid()), new KeyTypeName("OtherKey"));

        private static OrganisationTerminationSummary GetFieldsToTerminate(DateTime dateOfTermination, OrganisationState organisationState)
            => OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination, GetFieldsToTerminateConfig(), organisationState);

        private static OrganisationTerminationCalculator.FieldsToTerminateConfig GetFieldsToTerminateConfig()
            => new(RekenhofFormalFrameworkId, RekenhofCapacityId, RekenhofClassificationTypeId, VlimpersKeyTypeId);

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
                () => GetFieldsToTerminate(dateOfTermination, organisationState));
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

            GetFieldsToTerminate(dateOfTermination, organisationState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationSummaryBuilder()
                        .WithOrganisationNewValidTo(null)
                        .Build());
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

            GetFieldsToTerminate(dateOfTermination, organisationState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationSummaryBuilder()
                        .WithOrganisationNewValidTo(null)
                        .Build());
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

            GetFieldsToTerminate(dateOfTermination, organisationState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationSummaryBuilder()
                        .WithOrganisationNewValidTo(dateOfTermination)
                        .Build());
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
                () => GetFieldsToTerminate(dateOfTermination, organisationState));
        }

        [Fact]
        public void ThrowsWhenAnyRegulationValidityIsInFutureOfTerminationDate()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationState = new OrganisationState
            {
                OrganisationRegulations =
                {
                    fixture.Create<OrganisationRegulation>()
                        .WithValidity(new Period(
                            new ValidFrom(dateOfTermination.AddDays(1)),
                            new ValidTo()))
                }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => GetFieldsToTerminate(dateOfTermination, organisationState));
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
                () => GetFieldsToTerminate(dateOfTermination, organisationState));
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
                () => GetFieldsToTerminate(dateOfTermination, organisationState));
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
                () => GetFieldsToTerminate(dateOfTermination, organisationState));
        }

        [Fact]
        public void ThrowsWhenAnyFormalFrameworkToTerminateEndOfNextYearIsInFutureOfEndOfNextYear()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationFormalFramework = new OrganisationFormalFrameworkBuilder(fixture)
                .WithValidity(new ValidFrom(dateOfTermination.AddYears(2)), new ValidTo())
                .WithFormalFrameworkId(RekenhofFormalFrameworkId);

            var organisationState = new OrganisationState
            {
                OrganisationFormalFrameworks = { organisationFormalFramework }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => GetFieldsToTerminate(dateOfTermination, organisationState));
        }

        [Fact]
        public void ThrowsWhenAnyClassificationToTerminateEndOfNextYearIsInFutureOfEndOfNextYear()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationClassification = new OrganisationOrganisationClassificationBuilder(fixture)
                .WithValidity(new ValidFrom(dateOfTermination.AddYears(2)), new ValidTo())
                .WithOrganisationClassificationId(RekenhofClassificationTypeId);

            var organisationState = new OrganisationState
            {
                OrganisationOrganisationClassifications = { organisationClassification }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => GetFieldsToTerminate(dateOfTermination, organisationState));
        }

        [Fact]
        public void ThrowsWhenAnyCapacityToTerminateEndOfNextYearIsInFutureOfEndOfNextYear()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationCapacity = new OrganisationOrganisationCapacityBuilder(fixture)
                .WithOrganisationCapacityId(RekenhofCapacityId)
                .WithValidity(new ValidFrom(dateOfTermination.AddYears(2)), new ValidTo())
                .Build();

            var organisationState = new OrganisationState
            {
                OrganisationCapacities = { organisationCapacity }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => GetFieldsToTerminate(dateOfTermination, organisationState));
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
                OrganisationOpeningHours = { organisationOpeningHour }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => GetFieldsToTerminate(dateOfTermination, organisationState));
        }

        [Fact]
        public void ThrowsWhenAnyVlimpersKeyValidityIsInFutureOfTerminationDate()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationVlimpersKey = fixture.Create<OrganisationKey>()
                .WithKeyType(VlimpersKeyType)
                .WithValidity(new Period(
                    new ValidFrom(dateOfTermination.AddDays(1)),
                    new ValidTo()));

            var organisationState = new OrganisationState
            {
                OrganisationKeys = { organisationVlimpersKey }
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => GetFieldsToTerminate(dateOfTermination, organisationState));
        }

        [Fact]
        public void DoesNotThrowWhenAnyOtherKeyValidityIsInFutureOfTerminationDate()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationOtherKey = fixture.Create<OrganisationKey>()
                .WithKeyType(OtherKeyType)
                .WithValidity(new Period(
                    new ValidFrom(dateOfTermination.AddDays(1)),
                    new ValidTo()));

            var organisationState = new OrganisationState
            {
                OrganisationKeys = { organisationOtherKey }
            };

            var result = GetFieldsToTerminate(dateOfTermination, organisationState);

            result.Keys.Should().BeEmpty();
        }

        [Fact]
        public void OverwritesFieldsWhenValiditySpansOverDateOfTermination()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var overlappingContacts = fixture.CreateFieldsOverlappingWith<OrganisationContact>(dateOfTermination);
            var overlappingOpeningHours = fixture.CreateFieldsOverlappingWith<OrganisationOpeningHour>(dateOfTermination);
            var overlappingLabels = fixture.CreateFieldsOverlappingWith<OrganisationLabel>(dateOfTermination);
            var overlappingFunctions = fixture.CreateFieldsOverlappingWith<OrganisationFunction>(dateOfTermination);
            var overlappingLocations = fixture.CreateFieldsOverlappingWith<OrganisationLocation>(dateOfTermination);
            var overlappingBuildings = fixture.CreateFieldsOverlappingWith<OrganisationBuilding>(dateOfTermination);
            var overlappingParents = fixture.CreateFieldsOverlappingWith<OrganisationParent>(dateOfTermination);
            var overlappingBankAccounts = fixture.CreateFieldsOverlappingWith<OrganisationBankAccount>(dateOfTermination);
            var overlappingRelations = fixture.CreateFieldsOverlappingWith<OrganisationRelation>(dateOfTermination);
            var overlappingCapacities = fixture.CreateFieldsOverlappingWith<OrganisationCapacity>(dateOfTermination);
            var overlappingClassifications = fixture.CreateFieldsOverlappingWith<OrganisationOrganisationClassification>(dateOfTermination);
            var overlappingFormalFrameworks = fixture.CreateFieldsOverlappingWith<OrganisationFormalFramework>(dateOfTermination);
            var overlappingRegulations = fixture.CreateFieldsOverlappingWith<OrganisationRegulation>(dateOfTermination);

            var organisationState = new OrganisationState();
            organisationState.OrganisationContacts.AddRange(overlappingContacts);
            organisationState.OrganisationContacts.AddRange(fixture.CreateFieldsInThePastOf<OrganisationContact>(dateOfTermination));
            organisationState.OrganisationOpeningHours.AddRange(overlappingOpeningHours);
            organisationState.OrganisationOpeningHours.AddRange(fixture.CreateFieldsInThePastOf<OrganisationOpeningHour>(dateOfTermination));
            organisationState.OrganisationLabels.AddRange(overlappingLabels);
            organisationState.OrganisationLabels.AddRange(fixture.CreateFieldsInThePastOf<OrganisationLabel>(dateOfTermination));
            organisationState.OrganisationFunctionTypes.AddRange(overlappingFunctions);
            organisationState.OrganisationFunctionTypes.AddRange(fixture.CreateFieldsInThePastOf<OrganisationFunction>(dateOfTermination));
            organisationState.OrganisationLocations.AddRange(overlappingLocations);
            organisationState.OrganisationLocations.AddRange(fixture.CreateFieldsInThePastOf<OrganisationLocation>(dateOfTermination));
            organisationState.OrganisationBuildings.AddRange(overlappingBuildings);
            organisationState.OrganisationBuildings.AddRange(fixture.CreateFieldsInThePastOf<OrganisationBuilding>(dateOfTermination));
            organisationState.OrganisationParents.AddRange(overlappingParents);
            organisationState.OrganisationParents.AddRange(fixture.CreateFieldsInThePastOf<OrganisationParent>(dateOfTermination));
            organisationState.OrganisationBankAccounts.AddRange(overlappingBankAccounts);
            organisationState.OrganisationBankAccounts.AddRange(fixture.CreateFieldsInThePastOf<OrganisationBankAccount>(dateOfTermination));
            organisationState.OrganisationRelations.AddRange(overlappingRelations);
            organisationState.OrganisationRelations.AddRange(fixture.CreateFieldsInThePastOf<OrganisationRelation>(dateOfTermination));
            organisationState.OrganisationCapacities.AddRange(overlappingCapacities);
            organisationState.OrganisationCapacities.AddRange(fixture.CreateFieldsInThePastOf<OrganisationCapacity>(dateOfTermination));
            organisationState.OrganisationOrganisationClassifications.AddRange(overlappingClassifications);
            organisationState.OrganisationOrganisationClassifications.AddRange(fixture.CreateFieldsInThePastOf<OrganisationOrganisationClassification>(dateOfTermination));
            organisationState.OrganisationFormalFrameworks.AddRange(overlappingFormalFrameworks);
            organisationState.OrganisationFormalFrameworks.AddRange(fixture.CreateFieldsInThePastOf<OrganisationFormalFramework>(dateOfTermination));
            organisationState.OrganisationRegulations.AddRange(overlappingRegulations);
            organisationState.OrganisationRegulations.AddRange(fixture.CreateFieldsInThePastOf<OrganisationRegulation>(dateOfTermination));

            GetFieldsToTerminate(dateOfTermination,organisationState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationSummaryBuilder()
                        .WithOrganisationNewValidTo(new ValidTo(dateOfTermination))
                        .WithContacts(overlappingContacts.ToDictionary(x => x.OrganisationContactId, _ => dateOfTermination))
                        .WithOpeningHours(overlappingOpeningHours.ToDictionary(x => x.OrganisationOpeningHourId, _ => dateOfTermination))
                        .WithLabels(overlappingLabels.ToDictionary(x => x.OrganisationLabelId, _ => dateOfTermination))
                        .WithFunctions(overlappingFunctions.ToDictionary(x => x.OrganisationFunctionId, _ => dateOfTermination))
                        .WithLocations(overlappingLocations.ToDictionary(x => x.OrganisationLocationId, _ => dateOfTermination))
                        .WithBuildings(overlappingBuildings.ToDictionary(x => x.OrganisationBuildingId, _ => dateOfTermination))
                        .WithBankAccounts(overlappingBankAccounts.ToDictionary(x => x.OrganisationBankAccountId, _ => dateOfTermination))
                        .WithRelations(overlappingRelations.ToDictionary(x => x.OrganisationRelationId, _ => dateOfTermination))
                        .WithCapacities(overlappingCapacities.ToDictionary(x => x.OrganisationCapacityId, _ => dateOfTermination))
                        .WithClassifications(overlappingClassifications.ToDictionary(x => x.OrganisationOrganisationClassificationId, _ => dateOfTermination))
                        .WithFormalFrameworks(overlappingFormalFrameworks.ToDictionary(x => x.OrganisationFormalFrameworkId, _ => dateOfTermination))
                        .WithRegulations(overlappingRegulations.ToDictionary(x => x.OrganisationRegulationId, _ => dateOfTermination))
                        .Build()
                    );
        }

        [Fact]
        public void OverwritesOnlyVlimpersKeyFieldsWhenValiditySpansOverDateOfTermination()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var overlappingVlimpersKeys = fixture.CreateKeyFieldsOverlappingWith(dateOfTermination, VlimpersKeyType);
            var overlappingOtherKeys = fixture.CreateKeyFieldsOverlappingWith(dateOfTermination, OtherKeyType);

            var organisationState = new OrganisationState();

            organisationState.OrganisationKeys.AddRange(overlappingVlimpersKeys);
            organisationState.OrganisationKeys.AddRange(overlappingOtherKeys);
            organisationState.OrganisationKeys.AddRange(fixture.CreateKeyFieldsInThePastOf(dateOfTermination, VlimpersKeyType));
            organisationState.OrganisationKeys.AddRange(fixture.CreateKeyFieldsInThePastOf(dateOfTermination, OtherKeyType));

            GetFieldsToTerminate(dateOfTermination, organisationState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationSummaryBuilder()
                        .WithOrganisationNewValidTo(new ValidTo(dateOfTermination))
                        .WithKeys(overlappingVlimpersKeys.ToDictionary(x => x.OrganisationKeyId, _ => dateOfTermination))
                        .Build()
                    );
        }

        [Fact]
        public void OverwritesFieldsWhenValiditySpansOverEndOfNextYear()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var endOfNextYear = new DateTime(dateOfTermination.Year + 1, 12, 31);

            var capacityToTerminateEndOfNextYear = new OrganisationOrganisationCapacityBuilder(fixture)
                .WithOrganisationCapacityId(RekenhofCapacityId)
                .WithValidity(new ValidFrom(dateOfTermination.AddDays(1)), new ValidTo())
                .Build();
            var classificationToTerminateEndOfNextYear = new OrganisationOrganisationClassificationBuilder(fixture)
                .WithOrganisationClassificationId(RekenhofClassificationTypeId)
                .WithValidity(new ValidFrom(dateOfTermination.AddDays(1)), new ValidTo())
                .Build();
            var formalFrameworkToTerminateEndOfNextYear = new OrganisationFormalFrameworkBuilder(fixture)
                .WithFormalFrameworkId(RekenhofFormalFrameworkId)
                .WithValidity(new ValidFrom(dateOfTermination.AddDays(1)), new ValidTo())
                .Build();

            var capacitiesToTerminateEndOfNextYear = new List<OrganisationCapacity> { capacityToTerminateEndOfNextYear };
            var classificationsToTerminateEndOfNextYear = new List<OrganisationOrganisationClassification> { classificationToTerminateEndOfNextYear };
            var formalFrameworksToTerminateEndOfNextYear = new List<OrganisationFormalFramework> { formalFrameworkToTerminateEndOfNextYear };

            var organisationState = new OrganisationState();
            organisationState.OrganisationCapacities.AddRange(capacitiesToTerminateEndOfNextYear);
            organisationState.OrganisationCapacities.AddRange(fixture.CreateFieldsInThePastOf<OrganisationCapacity>(dateOfTermination));
            organisationState.OrganisationOrganisationClassifications.AddRange(classificationsToTerminateEndOfNextYear);
            organisationState.OrganisationOrganisationClassifications.AddRange(fixture.CreateFieldsInThePastOf<OrganisationOrganisationClassification>(dateOfTermination));
            organisationState.OrganisationFormalFrameworks.AddRange(formalFrameworksToTerminateEndOfNextYear);
            organisationState.OrganisationFormalFrameworks.AddRange(fixture.CreateFieldsInThePastOf<OrganisationFormalFramework>(dateOfTermination));

            GetFieldsToTerminate(dateOfTermination, organisationState)
                .Should().BeEquivalentTo(
                    new OrganisationTerminationSummaryBuilder()
                        .WithOrganisationNewValidTo(new ValidTo(dateOfTermination))
                        .WithCapacities(capacitiesToTerminateEndOfNextYear.ToDictionary(x => x.OrganisationCapacityId, _ => endOfNextYear))
                        .WithClassifications(classificationsToTerminateEndOfNextYear.ToDictionary(x => x.OrganisationOrganisationClassificationId, _ => endOfNextYear))
                        .WithFormalFrameworks(formalFrameworksToTerminateEndOfNextYear.ToDictionary(x => x.OrganisationFormalFrameworkId, _ => endOfNextYear))
                        .Build()
                    );
        }

        [Fact]
        public void OverwritesEndOfNextYearFieldsWhenTypeIdsMatch()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var overlappingCapacities = fixture.CreateFieldsOverlappingWith<OrganisationCapacity>(dateOfTermination);
            var overlappingClassifications = fixture.CreateFieldsOverlappingWith<OrganisationOrganisationClassification>(dateOfTermination);
            var overlappingFormalFrameworks = fixture.CreateFieldsOverlappingWith<OrganisationFormalFramework>(dateOfTermination);

            var organisationState = new OrganisationState();
            organisationState.OrganisationCapacities.AddRange(overlappingCapacities);
            organisationState.OrganisationCapacities.AddRange(fixture.CreateFieldsInThePastOf<OrganisationCapacity>(dateOfTermination));
            organisationState.OrganisationOrganisationClassifications.AddRange(overlappingClassifications);
            organisationState.OrganisationOrganisationClassifications.AddRange(fixture.CreateFieldsInThePastOf<OrganisationOrganisationClassification>(dateOfTermination));
            organisationState.OrganisationFormalFrameworks.AddRange(overlappingFormalFrameworks);
            organisationState.OrganisationFormalFrameworks.AddRange(fixture.CreateFieldsInThePastOf<OrganisationFormalFramework>(dateOfTermination));

            var result = GetFieldsToTerminate(dateOfTermination, organisationState);

            var expectation = new OrganisationTerminationSummaryBuilder()
                .WithOrganisationNewValidTo(new ValidTo(dateOfTermination))
                .WithCapacities(overlappingCapacities.ToDictionary(x => x.OrganisationCapacityId, _ => dateOfTermination))
                .WithClassifications(overlappingClassifications.ToDictionary(x => x.OrganisationOrganisationClassificationId, _ => dateOfTermination))
                .WithFormalFrameworks(overlappingFormalFrameworks.ToDictionary(x => x.OrganisationFormalFrameworkId, _ => dateOfTermination))
                .Build();

            result.Should().BeEquivalentTo(expectation);
        }

        [Fact]
        public void OverwritesKboFieldsWhenKboHasTermination()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var kboState = new KboState();
            kboState.KboBankAccounts.AddRange(fixture.CreateFieldsInThePastOf<OrganisationBankAccount>(dateOfTermination));
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

            OrganisationTerminationCalculator.GetKboFieldsToForceTerminate(dateOfTermination, kboState)
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
            kboState.KboBankAccounts.AddRange(fixture.CreateFieldsInThePastOf<OrganisationBankAccount>(dateOfTermination));
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

            OrganisationTerminationCalculator.GetKboFieldsToForceTerminate(dateOfTermination, kboState)
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
