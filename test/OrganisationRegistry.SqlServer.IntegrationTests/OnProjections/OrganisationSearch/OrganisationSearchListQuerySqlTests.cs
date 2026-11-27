namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationSearch;

using System.Linq;
using System.Text.RegularExpressions;
using Api.Backoffice.Organisation.List;
using Api.Infrastructure.Search.Filtering;
using Api.Infrastructure.Search.Pagination;
using Api.Infrastructure.Search.Sorting;
using FluentAssertions;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;

/// <summary>
/// De SQL van het overzicht op de hoofdpagina (OR-3785). Met een code-filter hoort er een tiebreaker op
/// <c>Id</c> achter de sortering: zonder die tiebreaker kiest SQL Server een plan dat bij een gerichte zoekterm
/// alle organisaties afloopt (zie <c>OrganisationSearchListQuery.Sort</c>). Een in-memory databank toont geen
/// uitvoeringsplan, dus wordt hier de gegenereerde SQL gecontroleerd.
/// </summary>
public class OrganisationSearchListQuerySqlTests
{
    [Theory]
    [InlineData(nameof(OrganisationSearchListQueryResult.Name))]
    [InlineData(nameof(OrganisationSearchListQueryResult.ParentOrganisation))]
    [InlineData(nameof(OrganisationSearchListQueryResult.OvoNumber))]
    public void SortsOnIdAsATiebreakerWhenFilteringOnCode(string sortBy)
    {
        var sql = PageSql(new OrganisationSearchListItemFilter { Code = "000" }, sortBy);

        OrderByClause(sql).Should().Be($"[o].[{sortBy}], [o].[Id]");
    }

    [Fact]
    public void KeepsTheRequestedSortingWithoutACodeFilter()
    {
        var sql = PageSql(new OrganisationSearchListItemFilter { Name = "Organisatie" }, nameof(OrganisationSearchListQueryResult.Name));

        OrderByClause(sql).Should().Be("[o].[Name]");
    }

    [Fact]
    public void TreatsAnEmptyCodeAsNoCodeFilter()
    {
        var sql = PageSql(new OrganisationSearchListItemFilter { Code = " . " }, nameof(OrganisationSearchListQueryResult.Name));

        OrderByClause(sql).Should().Be("[o].[Name]");
    }

    private static string OrderByClause(string sql)
        => Regex.Match(sql, @"ORDER BY (.+?)\s+OFFSET", RegexOptions.Singleline).Groups[1].Value;

    private static string PageSql(OrganisationSearchListItemFilter filter, string sortBy)
    {
        // ToQueryString maakt geen verbinding; de connectiestring wordt nooit gebruikt.
        using var context = new OrganisationRegistryContext(
            new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseSqlServer("Server=unused")
                .Options);

        var paged = new OrganisationSearchListQuery(context, new UserBuilder().Build(), new OrganisationRegistryConfigurationStub())
            .Fetch(
                new FilteringHeader<OrganisationSearchListItemFilter>(filter),
                new SortingHeader(sortBy, SortOrder.Ascending),
                new FirstPageWithoutCount());

        return paged.Items.ToQueryString();
    }

    /// <summary>De eerste pagina, zonder de telling uit te voeren: die zou wel een databankverbinding vragen.</summary>
    private sealed class FirstPageWithoutCount : IPaginationRequest
    {
        public PagedQueryable<T> Paginate<T>(IQueryable<T> source)
            => new(source.Skip(0).Take(10), new PaginationInfo(1, 10, 0, 0));
    }
}
