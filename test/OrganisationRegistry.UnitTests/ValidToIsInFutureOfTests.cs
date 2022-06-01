namespace OrganisationRegistry.UnitTests;

using System;
using FluentAssertions;
using Xunit;

public class ValidToIsInFutureOfTests
{
    [Fact]
    public void FutureValidTo_Is_InFutureOf()
    {
        new ValidTo(new DateTime(2017, 1, 2))
            .IsInFutureOf(new DateTime(2017, 1, 1))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void NullValidTo_Is_InFutureOf()
    {
        new ValidTo()
            .IsInFutureOf(new DateTime(2017, 1, 1))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void PastValidTo_IsNot_InFutureOf()
    {
        new ValidTo(new DateTime(2016, 1, 2))
            .IsInFutureOf(new DateTime(2017, 1, 1))
            .Should()
            .BeFalse();
    }

    [Fact]
    public void SameDayValidTo_IsNot_InFutureOfExclusive()
    {
        new ValidTo(new DateTime(2017, 1, 1))
            .IsInFutureOf(new DateTime(2017, 1, 1))
            .Should()
            .BeFalse();
    }

    [Fact]
    public void SameDayValidTo_Is_InFutureOfInclusive()
    {
        new ValidTo(new DateTime(2017, 1, 1))
            .IsInFutureOf(new DateTime(2017, 1, 1), true)
            .Should()
            .BeTrue();
    }
}
