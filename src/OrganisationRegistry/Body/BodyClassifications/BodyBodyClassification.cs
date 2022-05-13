namespace OrganisationRegistry.Body
{
    using System;

    public class BodyBodyClassification
    {
        public Guid BodyId { get; } // todo: remove bodyId from this (but not from event, possibly not from command)
        public Guid BodyBodyClassificationId { get; }
        public Guid BodyClassificationTypeId { get; }
        public string BodyClassificationTypeName { get; }
        public Guid BodyClassificationId { get; }
        public string BodyClassificationName { get; }
        public Period Validity { get; }

        public BodyBodyClassification(
            Guid bodyBodyClassificationId,
            Guid bodyId,
            Guid bodyClassificationTypeId,
            string bodyClassificationTypeName,
            Guid bodyClassificationId,
            string bodyClassificationName,
            Period validity)
        {
            BodyId = bodyId;
            BodyBodyClassificationId = bodyBodyClassificationId;
            BodyClassificationTypeId = bodyClassificationTypeId;
            BodyClassificationId = bodyClassificationId;
            Validity = validity;
            BodyClassificationTypeName = bodyClassificationTypeName;
            BodyClassificationName = bodyClassificationName;
        }
    }
}
