namespace OrganisationRegistry.Organisation;

using System;

public interface IKboLocationRetriever
{
    Guid? RetrieveLocation(IMagdaAddress address);
}