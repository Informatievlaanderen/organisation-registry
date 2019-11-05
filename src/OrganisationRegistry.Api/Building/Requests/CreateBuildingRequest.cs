namespace OrganisationRegistry.Api.Building.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.Building;
    using OrganisationRegistry.Building;
    using OrganisationRegistry.Building.Commands;

    public class CreateBuildingRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int? VimId { get; set; }
    }

    public class CreateBuildingRequestValidator : AbstractValidator<CreateBuildingRequest>
    {
        public CreateBuildingRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, BuildingListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {BuildingListConfiguration.NameLength}.");
        }
    }

    public static class CreateBuildingRequestMapping
    {
        public static CreateBuilding Map(CreateBuildingRequest message)
        {
            return new CreateBuilding(
                new BuildingId(message.Id),
                message.Name,
                message.VimId);
        }
    }
}
