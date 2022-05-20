namespace OrganisationRegistry.LabelType
{
    using System.Threading.Tasks;
    using Commands;
    using Exceptions;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class LabelTypeCommandHandlers :
        BaseCommandHandler<LabelTypeCommandHandlers>,
        ICommandEnvelopeHandler<CreateLabelType>,
        ICommandEnvelopeHandler<UpdateLabelType>
    {
        private readonly IUniqueNameValidator<LabelType> _uniqueNameValidator;

        public LabelTypeCommandHandlers(
            ILogger<LabelTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<LabelType> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(ICommandEnvelope<CreateLabelType> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                throw new NameNotUnique();

            var labelType = new LabelType(envelope.Command.LabelTypeId, envelope.Command.Name);
            Session.Add(labelType);
            await Session.Commit(envelope.User);
        }

        public async Task Handle(ICommandEnvelope<UpdateLabelType> envelope)
        {
            if (_uniqueNameValidator.IsNameTaken(envelope.Command.LabelTypeId, envelope.Command.Name))
                throw new NameNotUnique();

            var labelType = Session.Get<LabelType>(envelope.Command.LabelTypeId);
            labelType.Update(envelope.Command.Name);
            await Session.Commit(envelope.User);
        }
    }
}
