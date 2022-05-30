namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Collections.Generic;

public class OrganisationSecurityInformation
{
    public OrganisationSecurityInformation()
    {
        OvoNumbers = new List<string>();
        OrganisationIds = new List<Guid>();
        BodyIds = new List<Guid>();
    }

    public OrganisationSecurityInformation(
        IList<string> ovoNumbers,
        IList<Guid> organisationIds,
        IList<Guid> bodyIds)
    {
        OvoNumbers = ovoNumbers;
        OrganisationIds = organisationIds;
        BodyIds = bodyIds;
    }

    public IList<string> OvoNumbers { get; }

    public IList<Guid> OrganisationIds { get; }

    public IList<Guid> BodyIds { get; }
}