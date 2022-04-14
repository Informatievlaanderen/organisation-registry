namespace OrganisationRegistry
{
    using System;

    [Obsolete("This is a deleted event, do not use.", true)]
    public class BodyBecameActive : BaseEvent<BodyBecameActive>
    {
        public Guid BodyId => Id;

        public BodyBecameActive(Guid bodyId) => Id = bodyId;
    }

    [Obsolete("This is a deleted event, do not use.", true)]
    public class BodyBecameFormallyActive : BaseEvent<BodyBecameFormallyActive>
    {
        public Guid BodyId => Id;

        public BodyBecameFormallyActive(Guid bodyId) => Id = bodyId;
    }

    [Obsolete("This is a deleted event, do not use.", true)]
    public class BodyBecameInactive : BaseEvent<BodyBecameInactive>
    {
        public Guid BodyId => Id;

        public BodyBecameInactive(Guid bodyId) => Id = bodyId;
    }

    [Obsolete("This is a deleted event, do not use.", true)]
    public class BodyBecameFormallyInactive : BaseEvent<BodyBecameFormallyInactive>
    {
        public Guid BodyId => Id;

        public BodyBecameFormallyInactive(Guid bodyId) => Id = bodyId;
    }
}
