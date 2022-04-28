﻿namespace OrganisationRegistry.Api.Backoffice.Organisation.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ContactType;
    using FluentValidation;
    using Function;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Person;

    public class AddOrganisationFunctionInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public AddOrganisationFunctionRequest Body { get; }

        public AddOrganisationFunctionInternalRequest(Guid organisationId, AddOrganisationFunctionRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
        }
    }

    public class AddOrganisationFunctionRequest
    {
        public Guid OrganisationFunctionId { get; set; }
        public Guid FunctionId { get; set; }
        public Guid PersonId { get; set; }
        public Dictionary<Guid, string> Contacts { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class AddOrganisationFunctionInternalRequestValidator : AbstractValidator<AddOrganisationFunctionInternalRequest>
    {
        public AddOrganisationFunctionInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.FunctionId)
                .NotEmpty()
                .WithMessage("Function Id is required.");

            RuleFor(x => x.Body.PersonId)
                .NotEmpty()
                .WithMessage("Person Id is required.");

            // TODO: Validate if FunctionTypeId is valid

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");

            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Organisation Id is required.");

            // TODO: Validate if org id is valid
        }
    }

    public static class AddOrganisationFunctionRequestMapping
    {
        public static AddOrganisationFunction Map(AddOrganisationFunctionInternalRequest message)
        {
            return new AddOrganisationFunction(
                message.Body.OrganisationFunctionId,
                new OrganisationId(message.OrganisationId),
                new FunctionTypeId(message.Body.FunctionId),
                new PersonId(message.Body.PersonId),
                message.Body.Contacts?.ToDictionary(x => new ContactTypeId(x.Key), x => x.Value),
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
