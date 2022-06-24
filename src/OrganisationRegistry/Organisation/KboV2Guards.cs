namespace OrganisationRegistry.Organisation;

using System;
using Exceptions;
using Infrastructure.Configuration;
using LabelType;
using LocationType;
using OrganisationClassificationType;
using IOrganisationRegistryConfiguration = Infrastructure.Configuration.IOrganisationRegistryConfiguration;

public static class KboV2Guards
{
    public static void ThrowIfRegisteredOffice(
        IOrganisationRegistryConfiguration organisationRegistryConfiguration,
        LocationType? locationType)
    {
        if (organisationRegistryConfiguration.Kbo.KboV2RegisteredOfficeLocationTypeId == Guid.Empty)
            return;

        if (organisationRegistryConfiguration.Kbo.KboV2RegisteredOfficeLocationTypeId == locationType?.Id)
            throw new UserCannotCoupleKboRegisteredOffice();
    }

    public static void ThrowIfFormalName(IKboConfiguration kboConfiguration, LabelType labelType)
    {
        if (kboConfiguration.KboV2FormalNameLabelTypeId == Guid.Empty)
            return;

        if (kboConfiguration.KboV2FormalNameLabelTypeId == labelType.Id)
            throw new UserCannotCoupleKboLegalName();
    }

    public static void ThrowIfLegalForm(
        IOrganisationRegistryConfiguration organisationRegistryConfiguration,
        OrganisationClassificationType organisationClassificationType)
    {
        if (organisationRegistryConfiguration.Kbo.KboV2LegalFormOrganisationClassificationTypeId == Guid.Empty)
            return;

        if (organisationRegistryConfiguration.Kbo.KboV2LegalFormOrganisationClassificationTypeId == organisationClassificationType.Id)
            throw new UserCannotCoupleKboLegalFormOrganisationClassification();
    }

    public static void ThrowIfChanged(string? maybePreviousValue, string? maybeNewValue)
    {
        if (maybePreviousValue != maybeNewValue)
            throw new CannotChangeDataOwnedByKbo();
    }
}
