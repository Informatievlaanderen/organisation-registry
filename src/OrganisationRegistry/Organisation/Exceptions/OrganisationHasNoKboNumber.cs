namespace OrganisationRegistry.Organisation.Exceptions;

using System;

public class OrganisationHasNoKboNumber : DomainException
{
    public OrganisationHasNoKboNumber(Guid organisationId): base($"Organisation with Id {organisationId} has no KboNumber")
    {
    }
}
