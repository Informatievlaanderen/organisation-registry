namespace OrganisationRegistry.UnitTests.Security
{
    using System.Threading.Tasks;
    using Api.Security;
    using AutoFixture;
    using FluentAssertions;
    using OrganisationRegistry.Security;
    using Tests.Shared.Stubs;
    using Xunit;

    public class OrganisationSecurityCacheTests
    {
        private readonly Fixture _fixture;

        public OrganisationSecurityCacheTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeOrganisationSecurityInformation();
        }

        [Fact]
        public async Task WhenCacheIsEmpty_GetOrAdd_ReturnsNewItem()
        {
            var sut = new OrganisationSecurityCache(new OrganisationRegistryConfigurationStub());

            var expected = _fixture.Create<OrganisationSecurityInformation>();

            var key = _fixture.Create<string>();
            var actual = await sut.GetOrAdd(key, () => Task.FromResult(expected));

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task WhenCacheIsEmpty_GetOrAdd_ReturnsTheRightItem()
        {
            var sut = new OrganisationSecurityCache(new OrganisationRegistryConfigurationStub());

            var item1 = _fixture.Create<OrganisationSecurityInformation>();
            var item2 = _fixture.Create<OrganisationSecurityInformation>();

            var key1 = _fixture.Create<string>();
            var key2 = _fixture.Create<string>();
            await sut.GetOrAdd(key1, () => Task.FromResult(item1));
            await sut.GetOrAdd(key2, () => Task.FromResult(item2));
            var actual = await sut.GetOrAdd(key1, () => Task.FromResult(item1));

            actual.Should().Be(item1);
        }

        [Fact]
        public async Task WhenCacheIsNotEmpty_GetOrAdd_ReturnsItemFromCache()
        {
            var sut = new OrganisationSecurityCache(new OrganisationRegistryConfigurationStub());

            var item1 = _fixture.Create<OrganisationSecurityInformation>();
            var item2 = _fixture.Create<OrganisationSecurityInformation>();

            var key = _fixture.Create<string>();
            await sut.GetOrAdd(key, () => Task.FromResult(item1));
            var actual = await sut.GetOrAdd(key, () => Task.FromResult(item2));

            actual.Should().Be(item1);
        }

        [Fact]
        public async Task WhenCacheIsManuallyExpired_GetOrAdd_ReturnsNewItem()
        {
            var sut = new OrganisationSecurityCache(new OrganisationRegistryConfigurationStub());

            var item1 = _fixture.Create<OrganisationSecurityInformation>();
            var item2 = _fixture.Create<OrganisationSecurityInformation>();

            var key = _fixture.Create<string>();
            await sut.GetOrAdd(key, () => Task.FromResult(item1));

            sut.Expire(key);

            var actual = await sut.GetOrAdd(key, () => Task.FromResult(item2));

            actual.Should().Be(item2);
        }
    }
}
