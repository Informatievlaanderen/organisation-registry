namespace OrganisationRegistry.UnitTests.Organisation.Kbo
{
    using System;
    using OrganisationRegistry.Organisation;

    public class KboOrganisationClassificationRetrieverStub: IKboOrganisationClassificationRetriever
    {
        private readonly string _legalFormCodeToMatch;
        private readonly Guid _organisationClassificationIdToReturn;

        public KboOrganisationClassificationRetrieverStub(string legalFormCodeToMatch, Guid organisationClassificationIdToReturn)
        {
            _legalFormCodeToMatch = legalFormCodeToMatch;
            _organisationClassificationIdToReturn = organisationClassificationIdToReturn;
        }

        public Guid? FetchOrganisationClassificationForLegalFormCode(string legalFormCode)
        {
            if (_legalFormCodeToMatch == legalFormCode)
                return _organisationClassificationIdToReturn;

            return null;
        }
    }
}
