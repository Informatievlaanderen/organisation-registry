namespace OrganisationRegistry.UnitTests.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using Xunit;

public class RequireUnderVlimpersManagementRestrictionTests
{
    [Fact]
    public void Ok_when_context_reports_under_vlimpers_management()
    {
        var restriction = RequireUnderVlimpersManagementRestriction.Instance;

        restriction.IsOkWith(new KeyContext(true, Array.Empty<Guid>())).Should().BeTrue();
    }

    [Fact]
    public void Not_ok_when_context_reports_not_under_vlimpers_management()
    {
        var restriction = RequireUnderVlimpersManagementRestriction.Instance;

        restriction.IsOkWith(new KeyContext(false, Array.Empty<Guid>())).Should().BeFalse();
    }

    [Fact]
    public void Fails_closed_for_context_without_vlimpers_capability()
    {
        var restriction = RequireUnderVlimpersManagementRestriction.Instance;

        restriction.IsOkWith(new NonVlimpersContext()).Should().BeFalse();
    }

    [Fact]
    public void Instance_is_a_shared_singleton()
    {
        RequireUnderVlimpersManagementRestriction.Instance
            .Should().BeSameAs(RequireUnderVlimpersManagementRestriction.Instance);
    }

    [Fact]
    public void ToString_returns_stable_name()
    {
        RequireUnderVlimpersManagementRestriction.Instance.ToString()
            .Should().Be("RequireUnderVlimpersManagement");
    }

    private sealed class NonVlimpersContext : IRestrictionContext
    {
        public IEnumerable<Guid> RelevantIds => Array.Empty<Guid>();
    }
}
