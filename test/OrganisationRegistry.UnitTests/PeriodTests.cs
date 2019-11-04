namespace OrganisationRegistry.UnitTests
{
    using System;
    using FluentAssertions;
    using OrganisationRegistry.Organisation;
    using Xunit;

    public class PeriodTests
    {
        [Fact]
        public void PeriodsOverlapWhenTheyHaveTheSameBeginAndTheSameEnd()
        {
            var period1 = new Period(new ValidFrom(new DateTime(2016, 01, 01)), new ValidTo(new DateTime(2017, 01, 01)));
            var period2 = new Period(new ValidFrom(new DateTime(2016, 01, 01)), new ValidTo(new DateTime(2017, 01, 01)));

            period1.OverlapsWith(period2).Should().BeTrue();
            period2.OverlapsWith(period1).Should().BeTrue();
        }

        [Fact]
        public void APeriodOverlapsWhenItsBeginAndEndFallWithinTheOtherPeriod()
        {
            var period1 = new Period(new ValidFrom(new DateTime(2015, 01, 01)), new ValidTo(new DateTime(2018, 01, 01)));
            var period2 = new Period(new ValidFrom(new DateTime(2016, 01, 01)), new ValidTo(new DateTime(2017, 01, 01)));

            period1.OverlapsWith(period2).Should().BeTrue();
        }

        [Fact]
        public void APeriodOverlapsWhenTheOtherPeriodsBeginAndEndFallWithinItself()
        {
            var period1 = new Period(new ValidFrom(new DateTime(2015, 01, 01)), new ValidTo(new DateTime(2018, 01, 01)));
            var period2 = new Period(new ValidFrom(new DateTime(2016, 01, 01)), new ValidTo(new DateTime(2017, 01, 01)));

            period2.OverlapsWith(period1).Should().BeTrue();
        }

        [Fact]
        public void APeriodOverlapsWhenItsEndFallsBetweenTheOtherPeriodsBeginAndEnd()
        {
            var period1 = new Period(new ValidFrom(new DateTime(2015, 01, 01)), new ValidTo(new DateTime(2017, 01, 01)));
            var period2 = new Period(new ValidFrom(new DateTime(2016, 01, 01)), new ValidTo(new DateTime(2018, 01, 01)));

            period1.OverlapsWith(period2).Should().BeTrue();
        }

        [Fact]
        public void APeriodOverlapsWhenItsBeginFallsBetweenTheOtherPeriodsBeginAndEnd()
        {
            var period1 = new Period(new ValidFrom(new DateTime(2015, 01, 01)), new ValidTo(new DateTime(2017, 01, 01)));
            var period2 = new Period(new ValidFrom(new DateTime(2016, 01, 01)), new ValidTo(new DateTime(2018, 01, 01)));

            period2.OverlapsWith(period1).Should().BeTrue();
        }

        [Fact]
        public void APeriodOverlapsWhenItHasNoEndAndItsStartIsBeforeTheOtherPeriodsStart()
        {
            var period1 = new Period(new ValidFrom(new DateTime(2015, 01, 01)), new ValidTo());
            var period2 = new Period(new ValidFrom(new DateTime(2016, 01, 02)), new ValidTo(new DateTime(2017, 01, 01)));

            period1.OverlapsWith(period2).Should().BeTrue();
            period2.OverlapsWith(period1).Should().BeTrue();
        }


        [Fact]
        public void APeriodOverlapsWhenItHasNoStartAndItsEndIsAfterTheOtherPeriodsStart()
        {
            var period1 = new Period(new ValidFrom(), new ValidTo(new DateTime(2015, 01, 01)));
            var period2 = new Period(new ValidFrom(new DateTime(2014, 01, 02)), new ValidTo(new DateTime(2016, 01, 01)));

            period1.OverlapsWith(period2).Should().BeTrue();
            period2.OverlapsWith(period1).Should().BeTrue();
        }

        [Fact]
        public void APeriodDoesNotOverlapWhenItHasNoStartAndItsStartIsAfterTheOtherPeriodsStart()
        {
            var period1 = new Period(new ValidFrom(), new ValidTo(new DateTime(2015, 01, 01)));
            var period2 = new Period(new ValidFrom(new DateTime(2016, 01, 02)), new ValidTo(new DateTime(2017, 12, 31)));

            period1.OverlapsWith(period2).Should().BeFalse();
            period2.OverlapsWith(period1).Should().BeFalse();
        }

        [Fact]
        public void APeriodDoesNotOverlapWhenTheOtherPeriodsStartsAfterItsEndPeriod()
        {
            var period1 = new Period(new ValidFrom(new DateTime(2015, 01, 01)), new ValidTo(new DateTime(2016, 01, 01)));
            var period2 = new Period(new ValidFrom(new DateTime(2016, 01, 02)), new ValidTo(new DateTime(2017, 01, 01)));

            period2.OverlapsWith(period1).Should().BeFalse();
        }

        [Fact]
        public void APeriodDoesNotOverlapWhenTheOtherPeriodsEndsBeforeItsStartPeriod()
        {
            var period1 = new Period(new ValidFrom(new DateTime(2015, 01, 01)), new ValidTo(new DateTime(2016, 01, 01)));
            var period2 = new Period(new ValidFrom(new DateTime(2016, 01, 02)), new ValidTo(new DateTime(2017, 01, 01)));

            period1.OverlapsWith(period2).Should().BeFalse();
            period2.OverlapsWith(period1).Should().BeFalse();
        }

        [Fact]
        public void SomeoneDidNotBelieveMe()
        {
            var period1 = new Period(new ValidFrom(), new ValidTo());
            var period2 = new Period(new ValidFrom(new DateTime(2016, 01, 02)), new ValidTo(new DateTime(2017, 01, 01)));

            period1.OverlapsWith(period2).Should().BeTrue();
            period2.OverlapsWith(period1).Should().BeTrue();
        }

        [Fact]
        public void SomeoneStillDidntBelieveMe()
        {
            var period1 = new Period(new ValidFrom(), new ValidTo());
            var period2 = new Period(new ValidFrom(), new ValidTo());

            period1.OverlapsWith(period2).Should().BeTrue();
            period2.OverlapsWith(period1).Should().BeTrue();
        }

        [Fact]
        public void Nope()
        {
            var period1 = new Period(new ValidFrom(), new ValidTo(new DateTime(2000, 1, 1)));
            var period2 = new Period(new ValidFrom(new DateTime(1990, 1, 1)), new ValidTo());

            period1.OverlapsWith(period2).Should().BeTrue();
            period2.OverlapsWith(period1).Should().BeTrue();
        }

        [Fact]
        public void StillNot()
        {
            var period1 = new Period(new ValidFrom(), new ValidTo(new DateTime(2000, 1, 1)));
            var period2 = new Period(new ValidFrom(new DateTime(2010, 1, 1)), new ValidTo());

            period1.OverlapsWith(period2).Should().BeFalse();
            period2.OverlapsWith(period1).Should().BeFalse();
        }

        [Fact]
        public void AlmostGivingUp()
        {
            var period1 = new Period(new ValidFrom(), new ValidTo(new DateTime(2000, 1, 1)));
            var period2 = new Period(new ValidFrom(new DateTime(1900, 1, 1)), new ValidTo(new DateTime(2010, 1, 1)));

            period1.OverlapsWith(period2).Should().BeTrue();
            period2.OverlapsWith(period1).Should().BeTrue();
        }

        [Fact]
        public void CannotCreateAPeriodWithStartDateAfterEndDate()
        {
            Assert.Throws<StartDateCannotBeAfterEndDateException>(() => new Period(new ValidFrom(new DateTime(2000, 1, 2)), new ValidTo(new DateTime(2000, 1, 1))));
        }
    }
}
