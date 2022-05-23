namespace OrganisationRegistry
{
    using System;
    using Organisation.Exceptions;

    public class Period
    {
        public ValidFrom Start { get; }
        public ValidTo End { get; }

        public bool HasFixedStart => !Start.IsInfinite;
        public bool HasFixedEnd => !End.IsInfinite;

        public Period() { }

        public Period(ValidFrom start, ValidTo end)
        {
            var endDate = end.DateTime;
            var startDate = start.DateTime;
            if (endDate < startDate)
                throw new StartDateCannotBeAfterEndDate();

            Start = start;
            End = end;
        }

        public bool OverlapsWith(Period? period)
        {
            if (period == null)
                return false;

            var periodEndDate = period.End.DateTime;
            var startDate = Start.DateTime;
            if (periodEndDate < startDate)
                return false;

            var endDate = End.DateTime;
            var periodStartDate = period.Start.DateTime;
            if (endDate < periodStartDate)
                return false;

            return true;
        }

        public bool OverlapsWith(DateTime date)
            => OverlapsWith(
                new Period(
                    new ValidFrom(date),
                    new ValidTo(date)));

        public override string ToString()
            => $"{Start} => {End}";
    }
}
