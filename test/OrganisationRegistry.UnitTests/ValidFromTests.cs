// ReSharper disable EqualExpressionComparison
namespace OrganisationRegistry.UnitTests;

using System;
using FluentAssertions;
using Xunit;

public class ValidFromTests
{
    [Fact]
    public void WithSameDate_IsEqualTo()
    {
        new ValidFrom().Should().Be(new ValidFrom());
        new ValidFrom(new DateTime(2016, 1, 1).AddDays(2)).Should().Be(new ValidFrom(new DateTime(2016, 1, 1).AddDays(2)));
        new ValidFrom(2017, 1, 1).Should().Be(new ValidFrom(2017, 1, 1));
    }

    [Fact]
    public void WithDifferentDate_IsNotEqualTo()
    {
        new ValidFrom().Should().NotBe(new ValidFrom(2017, 1, 1));
        new ValidFrom(new DateTime(2017, 1, 1).AddDays(2)).Should().NotBe(new ValidFrom(new DateTime(2016, 1, 1).AddDays(2)));
        new ValidFrom(2018, 1, 1).Should().NotBe(new ValidFrom(2017, 1, 1));
    }

    [Fact]
    public void NullDate_IsSmallerThan_AnyDate()
    {
        (new ValidFrom(null) < new ValidFrom(DateTime.MinValue)).Should().BeTrue();
        (new ValidFrom(null) <= new ValidFrom(DateTime.MinValue)).Should().BeTrue();

        (new ValidFrom(DateTime.MaxValue) > new ValidFrom(null)).Should().BeTrue();
        (new ValidFrom(DateTime.MaxValue) >= new ValidFrom(null)).Should().BeTrue();
    }

    [Fact]
    public void RegularDates_UseRegularGreaterThan()
    {
        (new ValidFrom(DateTime.Now) > new ValidFrom(DateTime.Now.AddDays(-1))).Should().BeTrue();
        (new ValidFrom(DateTime.Now) >= new ValidFrom(DateTime.Now.AddDays(-1))).Should().BeTrue();

        (new ValidFrom(DateTime.Now.AddDays(-1)) < new ValidFrom(DateTime.Now)).Should().BeTrue();
        (new ValidFrom(DateTime.Now.AddDays(-1)) <= new ValidFrom(DateTime.Now)).Should().BeTrue();
    }

    [Fact]
    public void RegularDates_AreEqualTo()
    {
        (new ValidFrom(DateTime.Now.Date) == new ValidFrom(DateTime.Now.Date)).Should().BeTrue();

        new ValidFrom(DateTime.Now.Date).Equals(new ValidFrom(DateTime.Now.Date)).Should().BeTrue();
    }

    [Fact]
    public void RegularDates_AreNotEqualTo_NullDates()
    {
        (new ValidFrom(DateTime.Now.Date) == new ValidFrom()).Should().BeFalse();
        new ValidFrom(DateTime.Now.Date).Equals(new ValidFrom()).Should().BeFalse();
    }

    [Fact]
    public void NullDates_AreEqualTo_NullDates()
    {
        (new ValidFrom() == new ValidFrom()).Should().BeTrue();
        new ValidFrom().Equals(new ValidFrom()).Should().BeTrue();
    }
}