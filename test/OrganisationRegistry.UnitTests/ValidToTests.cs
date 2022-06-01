// ReSharper disable EqualExpressionComparison
namespace OrganisationRegistry.UnitTests;

using System;
using FluentAssertions;
using Xunit;

public class ValidToTests
{
    [Fact]
    public void WithSameDate_IsEqualTo()
    {
        new ValidTo().Should().Be(new ValidTo());
        new ValidTo(new DateTime(2016, 1, 1).AddDays(2)).Should().Be(new ValidTo(new DateTime(2016, 1, 1).AddDays(2)));
        new ValidTo(2017, 1, 1).Should().Be(new ValidTo(2017, 1, 1));
    }

    [Fact]
    public void WithDifferentDate_IsNotEqualTo()
    {
        new ValidTo().Should().NotBe(new ValidTo(2017, 1, 1));
        new ValidTo(new DateTime(2017, 1, 1).AddDays(2)).Should().NotBe(new ValidTo(new DateTime(2016, 1, 1).AddDays(2)));
        new ValidTo(2018, 1, 1).Should().NotBe(new ValidTo(2017, 1, 1));
    }

    [Fact]
    public void NullDate_IsGreaterThan_AnyDate()
    {
        (new ValidTo(null) > new ValidTo(DateTime.MaxValue)).Should().BeTrue();
        (new ValidTo(null) >= new ValidTo(DateTime.MaxValue)).Should().BeTrue();

        (new ValidTo(DateTime.MaxValue) < new ValidTo(null)).Should().BeTrue();
        (new ValidTo(DateTime.MaxValue) <= new ValidTo(null)).Should().BeTrue();
    }

    [Fact]
    public void RegularDates_UseRegularGreaterThan()
    {
        (new ValidTo(DateTime.Now) > new ValidTo(DateTime.Now.AddDays(-1))).Should().BeTrue();
        (new ValidTo(DateTime.Now) >= new ValidTo(DateTime.Now.AddDays(-1))).Should().BeTrue();

        (new ValidTo(DateTime.Now.AddDays(-1)) < new ValidTo(DateTime.Now)).Should().BeTrue();
        (new ValidTo(DateTime.Now.AddDays(-1)) <= new ValidTo(DateTime.Now)).Should().BeTrue();
    }

    [Fact]
    public void RegularDates_AreEqualTo()
    {
        (new ValidTo(DateTime.Now.Date) == new ValidTo(DateTime.Now.Date)).Should().BeTrue();

        new ValidTo(DateTime.Now.Date).Equals(new ValidTo(DateTime.Now.Date)).Should().BeTrue();
    }

    [Fact]
    public void RegularDates_AreNotEqualTo_NullDates()
    {
        (new ValidTo(DateTime.Now.Date) == new ValidTo()).Should().BeFalse();
        new ValidTo(DateTime.Now.Date).Equals(new ValidTo()).Should().BeFalse();
    }

    [Fact]
    public void NullDates_AreEqualTo_NullDates()
    {
        (new ValidTo() == new ValidTo()).Should().BeTrue();
        new ValidTo().Equals(new ValidTo()).Should().BeTrue();
    }
}
