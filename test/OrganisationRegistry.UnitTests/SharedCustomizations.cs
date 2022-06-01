namespace OrganisationRegistry.UnitTests;

using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation;

public static class SharedCustomizations
{
    public static void CustomizeOrganisationSecurityInformation(this IFixture fixture)
    {
        fixture.Customize<OrganisationSecurityInformation>(composer =>
            composer.FromFactory(generator =>
                new OrganisationSecurityInformation(
                    fixture.CreateMany<string>(generator.Next(0, 10)).ToList(),
                    fixture.CreateMany<Guid>(generator.Next(0, 10)).ToList(),
                    fixture.CreateMany<Guid>(generator.Next(0, 10)).ToList())
            ));
    }

    public static void CustomizeArticle(this IFixture fixture)
    {
        fixture.Customize<Article>(
            composer =>
                composer.FromFactory<int>(value => Article.All[Math.Abs(value) % Article.All.Length]));
    }

    public static void CustomizeValidFrom(this IFixture fixture)
    {
        fixture.Customize<ValidFrom>(
            composer =>
                composer.FromFactory(
                    generator => generator.Next() % 2 == 0
                        ? new ValidFrom(fixture.Create<DateTime?>())
                        : new ValidFrom()));
    }

    public static void CustomizeValidTo(this IFixture fixture)
    {
        fixture.Customize<ValidTo>(
            composer =>
                composer.FromFactory(
                    generator => generator.Next() % 2 == 0
                        ? new ValidTo(fixture.Create<DateTime?>())
                        : new ValidTo()));
    }

    public static void CustomizePeriod(this IFixture fixture)
    {
        CustomizeValidFrom(fixture);
        CustomizeValidTo(fixture);
        fixture.Customize<Period>(
            composer =>
                composer.FromFactory(
                    generator =>
                    {
                        var validFrom = fixture.Create<ValidFrom>();
                        var validTo = fixture.Create<ValidTo>();

                        if (!validFrom.IsInfinite && !validTo.IsInfinite &&
                            validTo.IsInPastOf(validFrom.DateTime!.Value))
                            validTo = new ValidTo(validFrom.DateTime.Value.AddDays(fixture.Create<int>()));

                        return new Period(validFrom, validTo);
                    }));
    }

    private static IPostprocessComposer<T> FromFactory<T>(
        this IFactoryComposer<T> composer,
        Func<Random, T> factory)
    {
        return composer.FromFactory<int>(value => factory(new Random(value)));
    }
}
