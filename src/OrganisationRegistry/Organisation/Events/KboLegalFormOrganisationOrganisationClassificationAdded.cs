namespace OrganisationRegistry.Organisation.Events
{
    using System;
    using Infrastructure;
    using Newtonsoft.Json;

    public class KboLegalFormOrganisationOrganisationClassificationAdded : BaseEvent<KboLegalFormOrganisationOrganisationClassificationAdded>
    {
        public Guid OrganisationId => Id;

        public Guid OrganisationOrganisationClassificationId { get; }
        public Guid OrganisationClassificationTypeId { get; }
        public string? OrganisationClassificationTypeName { get; }
        public Guid OrganisationClassificationId { get; }
        public string OrganisationClassificationName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public KboLegalFormOrganisationOrganisationClassificationAdded(
            Guid organisationId,
            Guid organisationOrganisationClassificationId,
            Guid organisationClassificationTypeId,
            string organisationClassificationTypeName,
            Guid organisationClassificationId,
            string organisationClassificationName,
            DateTime? validFrom,
            DateTime? validTo)
        {
            Id = organisationId;

            OrganisationOrganisationClassificationId = organisationOrganisationClassificationId;
            OrganisationClassificationTypeId = organisationClassificationTypeId;
            OrganisationClassificationTypeName = organisationClassificationTypeName;
            OrganisationClassificationId = organisationClassificationId;
            OrganisationClassificationName = organisationClassificationName;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        [JsonConstructor]
        [Obsolete("Do not use. This constructor exists solely to deserialize this event. " +
                  "We have both new and old versions of this event in the eventstore, " +
                  "one with organisationClassificationTypeName " +
                  "and one with classificationTypeName.")]
        public KboLegalFormOrganisationOrganisationClassificationAdded(
            Guid organisationId,
            Guid organisationOrganisationClassificationId,
            Guid organisationClassificationTypeId,
            string? organisationClassificationTypeName,
            Guid organisationClassificationId,
            string organisationClassificationName,
            DateTime? validFrom,
            DateTime? validTo,
            string? classificationTypeName = null)
        {
            Id = organisationId;

            OrganisationOrganisationClassificationId = organisationOrganisationClassificationId;
            OrganisationClassificationTypeId = organisationClassificationTypeId;
            OrganisationClassificationTypeName = GetClassificationTypeName(organisationClassificationTypeName, classificationTypeName);
            OrganisationClassificationId = organisationClassificationId;
            OrganisationClassificationName = organisationClassificationName;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        private static string? GetClassificationTypeName(string? maybeOrganisationClassificationTypeName, string? classificationTypeName)
            => maybeOrganisationClassificationTypeName is { } organisationClassificationTypeName && organisationClassificationTypeName.IsNotEmptyOrWhiteSpace()
                ? organisationClassificationTypeName
                : classificationTypeName;
    }
}
