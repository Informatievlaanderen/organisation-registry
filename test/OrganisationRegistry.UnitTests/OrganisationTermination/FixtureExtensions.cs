namespace OrganisationRegistry.UnitTests.OrganisationTermination;

using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using OrganisationRegistry.KeyTypes;
using OrganisationRegistry.Organisation;

public static class FixtureExtensions
{
    public static List<T> CreateFieldsInThePastOf<T>(this ISpecimenBuilder fixture, DateTime dateOfTermination) where T: IOrganisationField, IValidityBuilder<T>
        => fixture.CreateMany<T>()
            .Select(x => x.WithValidity(
                new Period(
                    new ValidFrom(),
                    new ValidTo(dateOfTermination.AddDays(fixture.Create<int>() * -1)))))
            .ToList();

    public static List<T> CreateFieldsOverlappingWith<T>(this ISpecimenBuilder fixture, DateTime dateOfTermination) where T: IOrganisationField, IValidityBuilder<T>
        => fixture.CreateMany<T>()
            .Select(x => x.WithValidity(
                new Period(
                    new ValidFrom(dateOfTermination.AddDays(fixture.Create<int>() * -1)),
                    new ValidTo(dateOfTermination.AddDays(fixture.Create<int>() * +1)))))
            .ToList();

    public static List<OrganisationKey> CreateKeyFieldsInThePastOf(this ISpecimenBuilder fixture, DateTime dateOfTermination, KeyType keyType)
        => fixture.CreateMany<OrganisationKey>()
            .Select(x => x.WithValidity(
                    new Period(
                        new ValidFrom(),
                        new ValidTo(dateOfTermination.AddDays(fixture.Create<int>() * -1))))
                .WithKeyType(keyType))
            .ToList();

    public static List<OrganisationKey> CreateKeyFieldsOverlappingWith(this ISpecimenBuilder fixture, DateTime dateOfTermination, KeyType keyType)
        => fixture.CreateMany<OrganisationKey>()
            .Select(
                x => x.WithValidity(
                        new Period(
                            new ValidFrom(dateOfTermination.AddDays(fixture.Create<int>() * -1)),
                            new ValidTo(dateOfTermination.AddDays(fixture.Create<int>() * +1))))
                    .WithKeyType(keyType))
            .ToList();
}
