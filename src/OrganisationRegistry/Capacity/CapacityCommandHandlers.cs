namespace OrganisationRegistry.Capacity
{
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class CapacityCommandHandlers :
        BaseCommandHandler<CapacityCommandHandlers>,
        ICommandHandler<CreateCapacity>,
        ICommandHandler<UpdateCapacity>
    {
        private readonly IUniqueNameValidator<Capacity> _uniqueNameValidator;

        public CapacityCommandHandlers(
            ILogger<CapacityCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<Capacity> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public void Handle(CreateCapacity message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var capacity = new Capacity(message.CapacityId, message.Name);
            Session.Add(capacity);
            Session.Commit();
        }

        public void Handle(UpdateCapacity message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.CapacityId, message.Name))
                throw new NameNotUniqueException();

            var capacity = Session.Get<Capacity>(message.CapacityId);
            capacity.Update(message.Name);
            Session.Commit();
        }
    }
}
