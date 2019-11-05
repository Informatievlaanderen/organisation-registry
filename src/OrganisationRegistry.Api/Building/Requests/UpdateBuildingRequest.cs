namespace OrganisationRegistry.Api.Building.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.Building;
    using OrganisationRegistry.Building;
    using OrganisationRegistry.Building.Commands;

    public class UpdateBuildingInternalRequest
    {
        public Guid BuildingId { get; set; }
        public UpdateBuildingRequest Body { get; set; }

        public UpdateBuildingInternalRequest(Guid buildingId, UpdateBuildingRequest body)
        {
            BuildingId = buildingId;
            Body = body;
        }
    }

    public class UpdateBuildingRequest
    {
        public string Name { get; set; }
        public int? VimId { get; set; }
    }

    public class UpdateBuildingRequestValidator : AbstractValidator<UpdateBuildingInternalRequest>
    {
        public UpdateBuildingRequestValidator()
        {
            RuleFor(x => x.BuildingId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, BuildingListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {BuildingListConfiguration.NameLength}.");
        }
    }

    public static class UpdateBuildingRequestMapping
    {
        public static UpdateBuilding Map(UpdateBuildingInternalRequest message)
        {
            return new UpdateBuilding(
                new BuildingId(message.BuildingId),
                message.Body.Name,
                message.Body.VimId);
        }
    }
}
