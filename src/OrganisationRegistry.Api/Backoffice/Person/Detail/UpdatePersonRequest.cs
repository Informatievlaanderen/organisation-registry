namespace OrganisationRegistry.Api.Backoffice.Person.Detail
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Person;
    using OrganisationRegistry.Person.Commands;
    using OrganisationRegistry.SqlServer.Person;

    public class UpdatePersonInternalRequest
    {
        public Guid PersonId { get; set; }
        public UpdatePersonRequest Body { get; set; }

        public UpdatePersonInternalRequest(Guid personId, UpdatePersonRequest body)
        {
            PersonId = personId;
            Body = body;
        }
    }

    public class UpdatePersonRequest
    {
        public string FirstName { get; set; }
        public string Name { get; set; }
        public Sex? Sex { get; set; } // TODO: We should map from non domain string to the enum
        public DateTime? DateOfBirth { get; set; }
    }

    public class UpdatePersonRequestValidator : AbstractValidator<UpdatePersonInternalRequest>
    {
        public UpdatePersonRequestValidator()
        {
            RuleFor(x => x.PersonId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.FirstName)
                .NotEmpty()
                .WithMessage("FirstName is required.");

            RuleFor(x => x.Body.FirstName)
                .Length(0, PersonListConfiguration.FirstNameLength)
                .WithMessage($"FirstName cannot be longer than {PersonListConfiguration.FirstNameLength}.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, PersonListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {PersonListConfiguration.NameLength}.");
        }
    }

    public static class UpdatePersonRequestMapping
    {
        public static UpdatePerson Map(UpdatePersonInternalRequest message)
            => new UpdatePerson(
                new PersonId(message.PersonId),
                new PersonFirstName(message.Body.FirstName),
                new PersonName(message.Body.Name),
                message.Body.Sex,
                message.Body.DateOfBirth);
    }
}
