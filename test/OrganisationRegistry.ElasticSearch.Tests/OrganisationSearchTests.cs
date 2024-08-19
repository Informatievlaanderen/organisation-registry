namespace OrganisationRegistry.ElasticSearch.Tests;

using FluentAssertions;
using Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Projections.Infrastructure;
using Scenario;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api;
using AutoFixture;
using Location;
using Location.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OpenSearch.Net;
using Organisation.Events;
using OrganisationRegistry.Tests.Shared;
using OrganisationRegistry.Tests.Shared.Stubs;
using Organisations;
using Osc;
using Projections.Organisations;
using Scenario.Specimen;
using SqlServer.Infrastructure;

[Collection(nameof(ElasticSearchFixture))]
public class OrganisationSearchTests
{
    private readonly ElasticSearchFixture _fixture;
    private readonly TestEventProcessor _eventProcessor;
    private readonly LocationUpdated? lastLocationEvent;

    public OrganisationSearchTests(ElasticSearchFixture fixture)
    {
        _fixture = fixture;
    }

    // [Fact]
    [Fact(Skip = "Manual only")]
    public async Task ElasticSearchClient_Search_With_Scroll_Timedout_Does_Not_Throw_Exception()
    {
        var client = _fixture.Elastic.ReadClient;
        var indexName = "scroll-timeout-test-index";

        try
        {
            var indexResponse = await _fixture.Elastic.WriteClient.Indices.CreateAsync(indexName);

            var scrollTimeout = "1s";
            var scroll = await client.SearchAsync<OrganisationDocument>(
                s => s
                    .Index(indexResponse.Index)
                    .From(0)
                    .Size(5)
                    .Scroll(scrollTimeout));

            await Task.Delay(5000);
            scroll = await client.ScrollAsync<OrganisationDocument>(scrollTimeout, scroll.ScrollId);

            Assert.Equal(scroll.ServerError.Error.Type, "search_phase_execution_exception");
        }
        finally
        {
            await _fixture.Elastic.WriteClient.Indices.DeleteAsync(indexName);
        }
    }
}
