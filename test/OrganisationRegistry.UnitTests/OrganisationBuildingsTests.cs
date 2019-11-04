namespace OrganisationRegistry.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Organisation;
    using Xunit;

    public class OrganisationBuildingsTests
    {
        [Fact]
        public void NewOrganisationBuildingsIsEmpty()
        {
            new OrganisationBuildings().Should().BeEmpty();
        }

        [Fact]
        public void NewOrganisationBuildingsFromIEnumerableAddsAllOrganisationBuildings()
        {
            new OrganisationBuildings(
                Enumerable.Repeat(new OrganisationBuildingTestDataBuilder().Build(), 10)
            ).Count.Should().Be(10);
        }

        [Fact]
        public void NewOrganisationBuildingsFromParamsAddsAllOrganisationBuildings()
        {
            new OrganisationBuildings(
                new OrganisationBuildingTestDataBuilder(),
                new OrganisationBuildingTestDataBuilder()
            ).Count.Should().Be(2);
        }

        [Fact]
        public void OrganisationAlreadyHasAMainBuildingInTheSamePeriod_WhenItHas()
        {
            var organisationBuilding =
                new OrganisationBuildingTestDataBuilder()
                .WithValidity(new Period(new ValidFrom(DateTime.Today), new ValidTo(DateTime.Today)))
                .WithMainBuilding(true).Build();

            var organisationBuildings = new OrganisationBuildings(organisationBuilding);

            var anotherOrganisationBuilding =
                new OrganisationBuildingTestDataBuilder()
                    .WithOrganisationId(organisationBuilding.OrganisationId)
                    .WithBuildingId(organisationBuilding.BuildingId)
                    .WithValidity(new Period(new ValidFrom(DateTime.Today), new ValidTo(DateTime.Today)))
                    .WithMainBuilding(true);

            organisationBuildings
                .OrganisationAlreadyHasAMainBuildingInTheSamePeriod(anotherOrganisationBuilding)
                .Should().BeTrue();
        }

        [Fact]
        public void OrganisationDoesNotHaveAMainBuildingInTheSamePeriod_WhenItHasnt()
        {
            var organisationBuilding =
                new OrganisationBuildingTestDataBuilder()
                .WithValidity(new Period(new ValidFrom(DateTime.Today), new ValidTo(DateTime.Today)))
                .WithMainBuilding(true).Build();

            var organisationBuildings = new OrganisationBuildings(organisationBuilding);

            var anotherOrganisationBuilding =
                new OrganisationBuildingTestDataBuilder()
                    .WithOrganisationId(organisationBuilding.OrganisationId)
                    .WithBuildingId(organisationBuilding.BuildingId)
                    .WithValidity(new Period(new ValidFrom(DateTime.Today.AddDays(1)), new ValidTo(DateTime.Today.AddDays(1))))
                    .WithMainBuilding(true);

            organisationBuildings
                .OrganisationAlreadyHasAMainBuildingInTheSamePeriod(anotherOrganisationBuilding)
                .Should().BeFalse();
        }

        [Fact]
        public void OrganisationAlreadyHasAMainBuildingInTheSamePeriod_DisregardsTheSameOrganisationBuilding()
        {
            var organisationBuilding =
                new OrganisationBuildingTestDataBuilder()
                .WithValidity(new Period(new ValidFrom(DateTime.Today), new ValidTo(DateTime.Today)))
                .WithMainBuilding(true).Build();

            var organisationBuildings = new OrganisationBuildings(organisationBuilding);

            organisationBuildings
                .OrganisationAlreadyHasAMainBuildingInTheSamePeriod(organisationBuilding)
                .Should().BeFalse();
        }
    }
}
