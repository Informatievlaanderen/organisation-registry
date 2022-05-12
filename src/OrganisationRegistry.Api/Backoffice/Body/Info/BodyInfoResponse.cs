namespace OrganisationRegistry.Api.Backoffice.Body.Info
{
    using System;
    using OrganisationRegistry.SqlServer.Body;

    public class BodyInfoResponse
    {
        public Guid Id { get; }

        public string BodyNumber { get; }
        public string Name { get; }
        public string? ShortName { get; }

        public string? Description { get; }

        public BodyInfoResponse(BodyDetail projectionItem)
        {
            Id = projectionItem.Id;

            BodyNumber = projectionItem.BodyNumber;
            Name = projectionItem.Name;
            ShortName = projectionItem.ShortName;
            Description = projectionItem.Description;
        }
    }
}
