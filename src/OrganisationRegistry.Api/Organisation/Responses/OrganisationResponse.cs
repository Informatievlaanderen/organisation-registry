namespace OrganisationRegistry.Api.Organisation.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SqlServer.Organisation;

    public class OrganisationResponse
    {
        public Guid Id { get; }

        public string OvoNumber { get; }

        public string? KboNumber { get; set; }

        public string Name { get; }
        public string? ShortName { get; }

        public string? ParentOrganisation { get; }
        public Guid? ParentOrganisationId { get; }

        public Guid? FormalFrameworkId { get; }
        public Guid? OrganisationClassificationId { get; }
        public Guid? OrganisationClassificationTypeId { get; }
        public string? Description { get; }
        public List<longstring> PurposeIds { get; }
        public List<string> Purposes { get; }
        public bool ShowOnVlaamseOverheidSites { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }
        public bool IsTerminated { get; set; }

        public OrganisationResponse(OrganisationDetailItem projectionItem)
        {
            Id = projectionItem.Id;
            OvoNumber = projectionItem.OvoNumber;
            KboNumber = projectionItem.KboNumber;
            Name = projectionItem.Name;
            ShortName = projectionItem.ShortName;
            ParentOrganisation = projectionItem.ParentOrganisation;
            ParentOrganisationId = projectionItem.ParentOrganisationId;
            FormalFrameworkId = projectionItem.FormalFrameworkId;
            OrganisationClassificationId = projectionItem.OrganisationClassificationId;
            OrganisationClassificationTypeId = projectionItem.OrganisationClassificationTypeId;
            Description = projectionItem.Description;
            PurposeIds = string.IsNullOrWhiteSpace(projectionItem.PurposeIds) ? new List<string>() : projectionItem.PurposeIds.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries).ToList();
            Purposes = string.IsNullOrWhiteSpace(projectionItem.PurposeNames) ? new List<string>() : projectionItem.PurposeNames.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ShowOnVlaamseOverheidSites = projectionItem.ShowOnVlaamseOverheidSites;
            ValidFrom = projectionItem.ValidFrom;
            ValidTo = projectionItem.ValidTo;
            IsTerminated = projectionItem.IsTerminated;
        }
    }
}
