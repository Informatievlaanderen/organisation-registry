namespace OrganisationRegistry.Organisation.Events
{
    using System;
    using System.Collections.Generic;

    public class OrganisationNameUpdated : BaseEvent<OrganisationNameUpdated>
    {
        public Guid OrganisationId => Id;

        public string Name { get; }

        public OrganisationNameUpdated(
            Guid organisationId,
            string name){
            Id = organisationId;

            Name = name;
        }
    }
    public class OrganisationArticleUpdated : BaseEvent<OrganisationArticleUpdated>
    {
        public Guid OrganisationId => Id;

        public string? Article { get; }

        public OrganisationArticleUpdated(
            Guid organisationId,
            string? article){
            Id = organisationId;

            Article = article;
        }
    }
    public class OrganisationDescriptionUpdated : BaseEvent<OrganisationDescriptionUpdated>
    {
        public Guid OrganisationId => Id;

        public string Description { get; }

        public OrganisationDescriptionUpdated(
            Guid organisationId,
            string description)
        {
            Id = organisationId;

            Description = description;
        }
    }
    public class OrganisationShortNameUpdated : BaseEvent<OrganisationShortNameUpdated>
    {
        public Guid OrganisationId => Id;

        public string ShortName { get; }

        public OrganisationShortNameUpdated(
            Guid organisationId,
            string shortName)
        {
            Id = organisationId;

            ShortName = shortName;
        }
    }
    public class OrganisationPurposesUpdated : BaseEvent<OrganisationPurposesUpdated>
    {
        public Guid OrganisationId => Id;

        public List<Purpose> Purposes { get; }

        public OrganisationPurposesUpdated(
            Guid organisationId,
            List<Purpose> purposes)
        {
            Id = organisationId;

            Purposes = purposes;
        }
    }
    public class OrganisationShowOnVlaamseOverheidSitesUpdated : BaseEvent<OrganisationShowOnVlaamseOverheidSitesUpdated>
    {
        public Guid OrganisationId => Id;
        public bool ShowOnVlaamseOverheidSites { get; }


        public OrganisationShowOnVlaamseOverheidSitesUpdated(
            Guid organisationId,
            bool showOnVlaamseOverheidSites)
        {
            Id = organisationId;

            ShowOnVlaamseOverheidSites = showOnVlaamseOverheidSites;
        }
    }
    public class OrganisationValidityUpdated : BaseEvent<OrganisationValidityUpdated>
    {
        public Guid OrganisationId => Id;

        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public OrganisationValidityUpdated(
            Guid organisationId,
            DateTime? validFrom,
            DateTime? validTo)
        {
            Id = organisationId;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
    public class OrganisationOperationalValidityUpdated : BaseEvent<OrganisationOperationalValidityUpdated>
    {
        public Guid OrganisationId => Id;
        public DateTime? OperationalValidFrom { get; }
        public DateTime? OperationalValidTo { get; }


        public OrganisationOperationalValidityUpdated(
            Guid organisationId,
            DateTime? operationalValidFrom,
            DateTime? operationalValidTo)
        {
            Id = organisationId;

            OperationalValidFrom = operationalValidFrom;
            OperationalValidTo = operationalValidTo;
        }
    }
}
