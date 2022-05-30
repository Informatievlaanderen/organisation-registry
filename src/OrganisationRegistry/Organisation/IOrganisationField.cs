namespace OrganisationRegistry.Organisation;

using System;

public interface IOrganisationField
{
    Guid Id { get; }
    Period Validity { get; }
}