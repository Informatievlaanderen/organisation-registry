namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.Person
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using TestBases;
    using OrganisationRegistry.Person.Events;
    using Xunit;
    using Sex = OrganisationRegistry.Person.Sex;

    [Collection(SqlServerTestsCollection.Name)]
    public class PersonListViewTests : ListViewTestBase
    {
        [Fact]
        public void PersonCreated()
        {
            var koenId = Guid.NewGuid();
            var davidId = Guid.NewGuid();
            HandleEvents(
                new PersonCreated(koenId, "Koen", "Metsu", Sex.Male, DateTime.Now.AddYears(-30)),
                new PersonCreated(davidId, "David", "Cumps", Sex.Male, DateTime.Now.AddYears(-31)));

            Context.PersonList.Single(item => item.Id == koenId).FirstName.Should().Be("Koen");
            Context.PersonList.Single(item => item.Id == davidId).FirstName.Should().Be("David");
        }

        [Fact]
        public void PersonUpdated()
        {
            var personId = Guid.NewGuid();
            HandleEvents(
                new PersonCreated(personId, "Koen", "Metsu", Sex.Male, DateTime.Now.AddYears(-30)),
                new PersonUpdated(personId, "Leander", "Metsu", Sex.Male, DateTime.Now.AddYears(-5), "Koen", "Metsu", Sex.Male, DateTime.Now.AddYears(-30)));

            Context.PersonList.Single(item => item.Id == personId).FirstName.Should().Be("Leander");
        }
    }
}
