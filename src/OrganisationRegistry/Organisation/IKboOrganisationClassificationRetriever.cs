namespace OrganisationRegistry.Organisation;

using System;

public interface IKboOrganisationClassificationRetriever
{
    Guid? FetchOrganisationClassificationForLegalFormCode(string legalFormCode);
}