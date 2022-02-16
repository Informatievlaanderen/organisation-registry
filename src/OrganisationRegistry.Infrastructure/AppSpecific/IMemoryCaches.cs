namespace OrganisationRegistry.Infrastructure.AppSpecific
{
    using System;
    using System.Collections.Generic;

    public interface IMemoryCaches
    {
        IReadOnlyDictionary<Guid, string> OvoNumbers { get; }
        IReadOnlyDictionary<Guid, string> OrganisationNames { get; }
        IReadOnlyDictionary<Guid, Guid?> OrganisationParents { get; }
        IReadOnlyDictionary<Guid, DateTime?> OrganisationValidFroms { get; }
        IReadOnlyDictionary<Guid, DateTime?> OrganisationValidTos { get; }

        IReadOnlyDictionary<Guid, string> BodyNames { get; }
        IReadOnlyDictionary<Guid, string> BodySeatNames { get; }
        IReadOnlyDictionary<Guid, string> BodySeatNumbers { get; }

        IReadOnlyDictionary<Guid, string> ContactTypeNames { get; }

        IReadOnlyDictionary<Guid, bool> IsSeatPaid { get; }
        IList<Guid> UnderVlimpersManagement { get; }

    }
}
