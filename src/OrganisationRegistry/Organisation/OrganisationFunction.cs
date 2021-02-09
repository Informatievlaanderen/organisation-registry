namespace OrganisationRegistry.Organisation
{
    using System;
    using System.Collections.Generic;

    public class OrganisationFunction : IOrganisationField, IValidityBuilder<OrganisationFunction>
    {
        public Guid Id => OrganisationFunctionId;
        public Guid OrganisationId { get; } // todo: remove organisationId from this (but not from event, possibly not from command)
        public Guid OrganisationFunctionId { get; }
        public Guid FunctionId { get; }
        public string FunctionName { get; }
        public Guid PersonId { get; }
        public string PersonName { get; }
        public Dictionary<Guid, string> Contacts { get; }
        public Period Validity { get; }

        public OrganisationFunction(
            Guid organisationFunctionId,
            Guid organisationId,
            Guid functionId,
            string functionName,
            Guid personId,
            string personName,
            Dictionary<Guid, string> contacts,
            Period validity)
        {
            OrganisationId = organisationId;
            OrganisationFunctionId = organisationFunctionId;
            FunctionId = functionId;
            FunctionName = functionName;
            PersonId = personId;
            PersonName = personName;
            Contacts = contacts;
            Validity = validity;
        }

        public OrganisationFunction WithValidity(Period period)
        {
            return new OrganisationFunction(
                OrganisationFunctionId,
                OrganisationId,
                FunctionId,
                FunctionName,
                PersonId,
                PersonName,
                Contacts,
                period);
        }

        public OrganisationFunction WithValidFrom(ValidFrom validFrom)
        {
            return WithValidity(new Period(validFrom, Validity.End));
        }

        public OrganisationFunction WithValidTo(ValidTo validTo)
        {
            return WithValidity(new Period(Validity.Start, validTo));
        }
    }
}
