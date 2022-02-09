namespace OrganisationRegistry.Api.Backoffice.Body.Responses
{
    using System;
    using SqlServer.Body;

    public class BodyResponse
    {
        public Guid Id { get; }

        public string BodyNumber { get; }
        public string Name { get; }
        public string ShortName { get; }

        public string Organisation { get; }
        public Guid? OrganisationId { get; }

        public string Description { get; }

        public DateTime? FormalValidFrom { get; }
        public DateTime? FormalValidTo { get; }

        public bool LifecycleValid { get; }
        public bool HasAllSeatsAssigned { get; }
        public bool IsMepCompliant { get; }

        public BodyResponse(
            BodyDetail projectionItem,
            bool hasAllSeatsAssigned,
            bool isMepCompliant)
        {
            Id = projectionItem.Id;

            BodyNumber = projectionItem.BodyNumber;
            Name = projectionItem.Name;
            ShortName = projectionItem.ShortName;
            Organisation = projectionItem.Organisation;
            OrganisationId = projectionItem.OrganisationId;
            Description = projectionItem.Description;
            FormalValidFrom = projectionItem.FormalValidFrom;
            FormalValidTo = projectionItem.FormalValidTo;
            LifecycleValid = projectionItem.IsLifecycleValid;

            HasAllSeatsAssigned = hasAllSeatsAssigned;
            IsMepCompliant = isMepCompliant;
        }
    }
}
