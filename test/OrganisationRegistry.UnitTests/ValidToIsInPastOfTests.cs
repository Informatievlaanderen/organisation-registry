namespace OrganisationRegistry.UnitTests;

using System;
using FluentAssertions;
using Xunit;

public class ValidToIsInPastOfTests
{
    [Fact]
    public void PastValidTo_Is_InPastOf()
    {
        new ValidTo(new DateTime(2016, 1, 1))
            .IsInPastOf(new DateTime(2017, 1, 1))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void NullValidTo_IsNot_InPastOf()
    {
        new ValidTo()
            .IsInPastOf(new DateTime(2017, 1, 1))
            .Should()
            .BeFalse();
    }

    [Fact]
    public void FutureValidTo_IsNot_InPastOf()
    {
        new ValidTo(new DateTime(2018, 1, 1))
            .IsInPastOf(new DateTime(2017, 1, 1))
            .Should()
            .BeFalse();
    }

    [Fact]
    public void SameDayValidTo_IsNot_InPastOfExclusive()
    {
        new ValidTo(new DateTime(2017, 1, 1))
            .IsInPastOf(new DateTime(2017, 1, 1))
            .Should()
            .BeFalse();
    }

    [Fact]
    public void SameDayValidTo_Is_InPastOfInclusive()
    {
        new ValidTo(new DateTime(2017, 1, 1))
            .IsInPastOf(new DateTime(2017, 1, 1), true)
            .Should()
            .BeTrue();
    }
}