namespace OrganisationRegistry.Api.IntegrationTests;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using OrganisationRegistry.Import.Piavo.Models;
using Newtonsoft.Json;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class WhenTheImportHasRun
{
    private readonly ApiFixture _fixture;

    public WhenTheImportHasRun(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<IEnumerable<T>> Get<T>(string requestUri)
    {
        var items = await _fixture.HttpClient.GetStringAsync(requestUri);
        return JsonConvert.DeserializeObject<IEnumerable<T>>(items) ?? new List<T>();
    }

    [Fact]
    public async Task ThereAreBuildings()
    {
        (await Get<BuildingListItem>("buildings"))
            .Count()
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task ThereAreCapacities()
    {
        (await Get<CapacityListItem>("capacities"))
            .Count()
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task ThereAreContactTypes()
    {
        (await Get<OrganisationContactListItem>("contacttypes"))
            .Count()
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task ThereAreKeyTypes()
    {
        (await Get<OrganisationKeyListItem>("keytypes"))
            .Count()
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task ThereAreLabelTypes()
    {
        (await Get<OrganisationLabelListItem>("labeltypes"))
            .Count()
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task ThereAreOrganisationClassifications()
    {
        (await Get<OrganisationClassificationListItem>("organisationclassifications"))
            .Count()
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task ThereAreOrganisationClassificationTypes()
    {
        (await Get<OrganisationClassificationTypeListItem>("organisationclassificationtypes"))
            .Count()
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task ThereAreOrganisations()
    {
        (await Get<OrganisationListQueryResult>("organisations"))
            .Count()
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task ThereAreFunctions()
    {
        (await Get<FunctionListItem>("functiontypes"))
            .Count()
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task ThereArePeople()
    {
        (await Get<PersonListItem>("people"))
            .Count()
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task AtLeastOneOrganisationHasCapacities()
    {
        (await Get<OrganisationListQueryResult>("organisations"))
            .Count(item => Get<OrganisationCapacityListItem>($"organisations/{item.Id}/capacities").GetAwaiter().GetResult().Any())
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task AtLeastOneOrganisationHasChildren()
    {
        (await Get<OrganisationListQueryResult>("organisations"))
            .Count(item => Get<object>($"organisations/{item.Id}/children").GetAwaiter().GetResult().Any())
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task AtLeastOneOrganisationHasContacts()
    {
        (await Get<OrganisationListQueryResult>("organisations"))
            .Count(item => Get<OrganisationContactListItem>($"organisations/{item.Id}/contacts").GetAwaiter().GetResult().Any())
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task AtLeastOneOrganisationHasKeys()
    {
        (await Get<OrganisationListQueryResult>("organisations"))
            .Count(item => Get<OrganisationKeyListItem>($"organisations/{item.Id}/keys").GetAwaiter().GetResult().Any())
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task AtLeastOneOrganisationHasClassifications()
    {
        (await Get<OrganisationListQueryResult>("organisations"))
            .Count(item => Get<OrganisationOrganisationClassificationListItem>($"organisations/{item.Id}/classifications").GetAwaiter().GetResult().Any())
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public async Task AtLeastOnePersonHasCapacities()
    {
        (await Get<PersonListItem>("people"))
            .Count(item => Get<object>($"people/{item.Id}/capacities").GetAwaiter().GetResult().Any())
            .Should()
            .BeGreaterThan(0);
    }
}