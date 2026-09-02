namespace OrganisationRegistry.UnitTests.Authorization.Restrictions;

using System;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using Xunit;

public class SingletonRestrictionTests
{
    private static readonly Guid A = Guid.NewGuid();

    [Fact]
    public void Unrestricted_always_allows()
    {
        var unrestricted = UnrestrictedRestriction<KeyRestrictionContext>.Instance;

        unrestricted.IsOkWith(new KeyRestrictionContext(A)).Should().BeTrue();
        unrestricted.IsOkWith(new KeyRestrictionContext(Array.Empty<Guid>())).Should().BeTrue();
    }

    [Fact]
    public void DenyAll_never_allows()
    {
        var denyAll = DenyAllRestriction<KeyRestrictionContext>.Instance;

        denyAll.IsOkWith(new KeyRestrictionContext(A)).Should().BeFalse();
        denyAll.IsOkWith(new KeyRestrictionContext(Array.Empty<Guid>())).Should().BeFalse();
    }

    [Fact]
    public void Instances_are_singletons_per_context()
    {
        UnrestrictedRestriction<KeyRestrictionContext>.Instance
            .Should().BeSameAs(UnrestrictedRestriction<KeyRestrictionContext>.Instance);
        DenyAllRestriction<KeyRestrictionContext>.Instance
            .Should().BeSameAs(DenyAllRestriction<KeyRestrictionContext>.Instance);
    }

    [Fact]
    public void Domains_match_context_domain()
    {
        UnrestrictedRestriction<KeyRestrictionContext>.Instance.Domain
            .Should().Be(KeyRestrictionContext.Domain);
        DenyAllRestriction<KeyRestrictionContext>.Instance.Domain
            .Should().Be(KeyRestrictionContext.Domain);
    }
}
