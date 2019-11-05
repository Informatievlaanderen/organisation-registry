namespace OrganisationRegistry.Api.Person.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.Person;
    using OrganisationRegistry.Person;
    using OrganisationRegistry.Person.Commands;
    using Sex = OrganisationRegistry.Person.Sex;

    public class CreatePersonRequest
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string Name { get; set; }

        public Sex? Sex { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }

    public class CreatePersonRequestValidator : AbstractValidator<CreatePersonRequest>
    {
        public CreatePersonRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("FirstName is required.");

            RuleFor(x => x.FirstName)
                .Length(0, PersonListConfiguration.FirstNameLength)
                .WithMessage($"FirstName cannot be longer than {PersonListConfiguration.FirstNameLength}.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, PersonListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {PersonListConfiguration.NameLength}.");
        }
    }

    public static class CreatePersonRequestMapping
    {
        public static CreatePerson Map(CreatePersonRequest message)
        {
            return new CreatePerson(
                new PersonId(message.Id),
                message.FirstName,
                message.Name,
                message.Sex,
                message.DateOfBirth);
        }
    }
}
