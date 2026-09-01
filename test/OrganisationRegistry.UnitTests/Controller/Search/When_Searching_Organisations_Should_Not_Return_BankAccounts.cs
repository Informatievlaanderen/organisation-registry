namespace OrganisationRegistry.UnitTests.Controller.Search;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api;
using Api.Search;
using AutoFixture;
using ElasticSearch.Client;
using ElasticSearch.Organisations;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

public class When_Searching_Organisations_Should_Not_Return_BankAccounts
{
    private readonly Fixture _fixture;
    private readonly string _bankAccountNumberToFilterAway;
    private readonly MockSearchResponse _searchResponse;

    public When_Searching_Organisations_Should_Not_Return_BankAccounts()
    {
        _fixture = new Fixture();

        var organisationBankAccounts = _fixture.CreateMany<OrganisationDocument.OrganisationBankAccount>().ToList();
        _bankAccountNumberToFilterAway = organisationBankAccounts.First().BankAccountNumber;

        _searchResponse = new MockSearchResponse(
        [
            new MockHit(
                new OrganisationDocument
                {
                    BankAccounts = organisationBankAccounts,
                }),
        ]);

        _searchResponse.Hits.First().Source.BankAccounts.First().BankAccountNumber.Should().NotBeNullOrEmpty();
    }

    private SearchController CreateController(Mock<IElasticSearchFacade> searchFacade)
    {
        var controller = new SearchController(searchFacade.Object, new Microsoft.Extensions.Logging.Abstractions.NullLogger<SearchController>(), Mock.Of<IHttpContextAccessor>());
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext(),
        };
        return controller;
    }

    [Fact]
    public async Task On_GetApiSearch()
    {
        var searchFacade = new Mock<IElasticSearchFacade>();
        searchFacade.Setup(x => x.SearchOrganisations(
                It.IsAny<Elastic>(),
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_searchResponse);

        var controller = CreateController(searchFacade);

        var result = await controller.GetApiSearch(ElasticSearchFacade.OrganisationsIndexName, null, null, _fixture.Create<string>(), null, null, "", "", false);

        ((ContentResult)result).Content.Should().NotContain(_bankAccountNumberToFilterAway);
    }

    [Fact]
    public async Task On_GetSearch()
    {
        var searchFacade = new Mock<IElasticSearchFacade>();
        searchFacade.Setup(x => x.SearchOrganisations(
                It.IsAny<Elastic>(),
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_searchResponse);

        var controller = CreateController(searchFacade);

        var result = await controller.GetSearch(ElasticSearchFacade.OrganisationsIndexName, null, _fixture.Create<string>(), 1, 1, _fixture.Create<string>(), "name", false);

        ((ContentResult)result).Content.Should().NotContain(_bankAccountNumberToFilterAway);
    }

    [Fact]
    public async Task On_PostApiSearch()
    {
        var searchFacade = new Mock<IElasticSearchFacade>();
        searchFacade.Setup(x => x.PostApiSearchOrganisations(
                It.IsAny<Elastic>(),
                It.IsAny<JObject>(),
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_searchResponse);

        var controller = CreateController(searchFacade);

        var result = await controller.PostApiSearch(ElasticSearchFacade.OrganisationsIndexName, null, _fixture.Create<JObject>(), 1, 1, _fixture.Create<string>(), "name", false);

        ((ContentResult)result).Content.Should().NotContain(_bankAccountNumberToFilterAway);
    }

    [Fact]
    public async Task On_ScrollApiSearch()
    {
        var searchFacade = new Mock<IElasticSearchFacade>();
        searchFacade.Setup(x => x.ScrollApiSearch<OrganisationDocument>(
                It.IsAny<Elastic>(),
                It.IsAny<string>()))
            .ReturnsAsync(_searchResponse);

        var controller = CreateController(searchFacade);

        var result = await controller.ScrollApiSearch(ElasticSearchFacade.OrganisationsIndexName, null, _fixture.Create<string>());

        ((ContentResult)result).Content.Should().NotContain(_bankAccountNumberToFilterAway);
    }
}

