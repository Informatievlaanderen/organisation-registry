namespace OrganisationRegistry.UnitTests
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    public class OrganisationOrganisationClassificationTests
    {
        private string newVersion =
"{\"organisationId\":\"2b4246ba-08ac-4b96-9a36-80d43bc3fc0a\",\"organisationOrganisationClassificationId\":\"39cf1df1-c6ad-3b0b-ab6d-11f74f8e61a4\",\"organisationClassificationTypeId\":\"4250299c-e709-fad5-9c5b-04341392c89f\",\"organisationClassificationTypeName\":\"Uitvoerend niveau\",\"organisationClassificationId\":\"e402b8c5-261f-6dc9-bb35-f29f0e894811\",\"organisationClassificationName\":\"Vlaamse administratie\",\"validFrom\":null,\"validTo\":null,\"version\":140,\"timestamp\":\"2017-08-29T07:28:36Z\"}";

        private string oldVersion =
"{\"organisationId\":\"2b4246ba-08ac-4b96-9a36-80d43bc3fc0a\",\"organisationOrganisationClassificationId\":\"e0f36106-1070-4400-bd39-9ab2ffbc4814\",\"organisationClassificationTypeId\":\"1131205e-9212-435d-b4cd-b0d955d08bcf\",\"classificationTypeName\":\"Juridische vorm\",\"organisationClassificationId\":\"d4016a8f-42ce-4629-8e27-2e8ef77bd15f\",\"organisationClassificationName\":\"Ministerie van de Vlaamse Gemeenschap (MVG)\",\"validFrom\":\"2006-04-01\",\"validTo\":null,\"version\":14,\"timestamp\":\"2017-01-18T22:24:13Z\",\"ip\":\"81.82.247.56\",\"firstName\":\"John\",\"lastName\":\"Admin\",\"userId\":null}";

        [Fact]
        public void NewVersion()
        {
            var @event = (OrganisationOrganisationClassificationAdded?) JsonConvert.DeserializeObject(newVersion, typeof(OrganisationOrganisationClassificationAdded));
            @event.Should().NotBeNull();
            @event!.OrganisationClassificationTypeName.Should().Be("Uitvoerend niveau");
        }

        [Fact]
        public void OldVersion()
        {
            var @event = (OrganisationOrganisationClassificationAdded?) JsonConvert.DeserializeObject(oldVersion, typeof(OrganisationOrganisationClassificationAdded));
            @event.Should().NotBeNull();
            @event!.OrganisationClassificationTypeName.Should().Be("Juridische vorm");
        }
    }
}
