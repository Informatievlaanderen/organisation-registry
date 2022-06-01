namespace OrganisationRegistry.UnitTests;

using System;
using FluentAssertions;
using Xunit;

public class IsInFutureOfTests
{
    [Fact]
    public void FutureValidFrom_Is_InFutureOf()
    {
        new ValidFrom(new DateTime(2017, 1, 2))
            .IsInFutureOf(new DateTime(2017, 1, 1))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void NullValidFrom_IsNot_InFutureOf()
    {
        new ValidFrom()
            .IsInFutureOf(new DateTime(2017, 1, 1))
            .Should()
            .BeFalse();
    }

    [Fact]
    public void PastValidFrom_IsNot_InFutureOf()
    {
        new ValidFrom(new DateTime(2016, 1, 2))
            .IsInFutureOf(new DateTime(2017, 1, 1))
            .Should()
            .BeFalse();
    }

    [Fact]
    public void SameDayValidFrom_IsNot_InFutureOfExclusive()
    {
        new ValidFrom(new DateTime(2017, 1, 1))
            .IsInFutureOf(new DateTime(2017, 1, 1))
            .Should()
            .BeFalse();
    }

    [Fact]
    public void SameDayValidFrom_Is_InFutureOfInclusive()
    {
        new ValidFrom(new DateTime(2017, 1, 1))
            .IsInFutureOf(new DateTime(2017, 1, 1), true)
            .Should()
            .BeTrue();
    }
}
