namespace OrganisationRegistry.Organisation
{
    using System;
    using Exceptions;
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

        public static void ThrowIfFormalName(
            IOrganisationRegistryConfiguration organisationRegistryConfiguration,
            LabelType labelType)
        {
            if (organisationRegistryConfiguration.Kbo.KboV2FormalNameLabelTypeId == Guid.Empty)
                return;

            if (organisationRegistryConfiguration.Kbo.KboV2FormalNameLabelTypeId == labelType.Id)
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

        public static void ThrowIfChanged(string previousValue, string newValue)
        {
            if (previousValue != newValue)
                throw new CannotChangeDataOwnedByKbo();
        }
    }
}
