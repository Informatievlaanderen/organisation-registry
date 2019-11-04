namespace OrganisationRegistry.Day
{
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Infrastructure.Domain.Exception;
    using Microsoft.Extensions.Logging;

    public class DayCommandHandlers :
        BaseCommandHandler<DayCommandHandlers>,
        ICommandHandler<CheckIfDayHasPassed>
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public DayCommandHandlers(
            ILogger<DayCommandHandlers> logger,
            ISession session,
            IDateTimeProvider dateTimeProvider) : base(logger, session)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public void Handle(CheckIfDayHasPassed message)
        {
            try
            {
                var day = Session.Get<Day>(message.DayId);
                day.CheckIfDayHasPassed(_dateTimeProvider);
            }
            catch (AggregateNotFoundException)
            {
                var day = new Day(_dateTimeProvider, message.DayId);
                Session.Add(day);
            }

            Session.Commit();
        }
    }
}
