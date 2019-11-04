namespace OrganisationRegistry.UnitTests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class IsInPastOfTests
    {
        [Fact]
        public void PastValidFrom_Is_IsInPastOf()
        {
            new ValidFrom(new DateTime(2016, 1, 1))
                .IsInPastOf(new DateTime(2017, 1, 1))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void NullValidFrom_Is_IsInPastOf()
        {
            new ValidFrom()
                .IsInPastOf(new DateTime(2017, 1, 1))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void FutureValidFrom_IsNot_IsInPastOf()
        {
            new ValidFrom(new DateTime(2018, 1, 1))
                .IsInPastOf(new DateTime(2017, 1, 1))
                .Should()
                .BeFalse();
        }

        [Fact]
        public void SameDayValidFrom_IsNot_IsInPastOfExclusive()
        {
            new ValidFrom(new DateTime(2017, 1, 1))
                .IsInPastOf(new DateTime(2017, 1, 1))
                .Should()
                .BeFalse();
        }

        [Fact]
        public void SameDayValidFrom_Is_IsInPastOfInclusive()
        {
            new ValidFrom(new DateTime(2017, 1, 1))
                .IsInPastOf(new DateTime(2017, 1, 1), true)
                .Should()
                .BeTrue();
        }
    }
}
