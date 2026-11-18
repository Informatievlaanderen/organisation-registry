namespace OrganisationRegistry.ArchitectureTests;

using System;
using System.Collections.Generic;
using System.Linq;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer.Organisation;
using Xunit;

/// <summary>
/// Waarom: OrganisationSearchList is een bewuste kopie van OrganisationList (OR-3785). Zonder
/// waakhond raakt die kopie stil achterop zodra iemand een event toevoegt aan het origineel, en
/// vertoont de hoofdpagina ander gedrag dan de rest van de applicatie.
/// </summary>
public class OrganisationSearchProjectionDriftTests
{
    [Fact]
    public void OrganisationSearchListHandlesAtLeastTheEventsOfOrganisationList()
    {
        var original = HandledEventTypes(typeof(OrganisationListItemView));
        var copy = HandledEventTypes(typeof(OrganisationSearchListItemView));

        Assert.NotEmpty(original);

        var missing = original.Except(copy).Select(type => type.Name).OrderBy(name => name).ToList();

        Assert.True(
            missing.Count == 0,
            $"OrganisationSearchListItemView handelt deze events van OrganisationListItemView niet af: {string.Join(", ", missing)}");
    }

    /// <summary>
    /// De codetabel voegt events toe die het origineel niet kent; die mogen er zijn, maar zijn
    /// hier expliciet opgesomd zodat een onbedoelde uitbreiding opvalt.
    /// </summary>
    [Fact]
    public void OrganisationSearchListOnlyAddsTheEventsNeededForCodes()
    {
        var original = HandledEventTypes(typeof(OrganisationListItemView));
        var copy = HandledEventTypes(typeof(OrganisationSearchListItemView));

        var additional = copy.Except(original).Select(type => type.Name).OrderBy(name => name).ToList();

        var expected = new[]
        {
            "KeyTypeRemoved",
            "OrganisationCoupledWithKbo",
            "OrganisationKeyAdded",
            "OrganisationKeyRemoved",
            "OrganisationKeyUpdated",
        };

        Assert.Equal(expected, additional);
    }

    private static IReadOnlyCollection<Type> HandledEventTypes(Type projection)
        => projection
            .GetInterfaces()
            .Where(@interface => @interface.IsGenericType)
            .Where(@interface => @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>))
            .Select(@interface => @interface.GetGenericArguments().Single())
            .Where(eventType => eventType != typeof(RebuildProjection))
            .ToList();
}
