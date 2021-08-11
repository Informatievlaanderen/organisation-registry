namespace OrganisationRegistry.LabelType
{
    using System.Threading.Tasks;
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class LabelTypeCommandHandlers :
        BaseCommandHandler<LabelTypeCommandHandlers>,
        ICommandHandler<CreateLabelType>,
        ICommandHandler<UpdateLabelType>
    {
        private readonly IUniqueNameValidator<LabelType> _uniqueNameValidator;

        public LabelTypeCommandHandlers(
            ILogger<LabelTypeCommandHandlers> logger,
            ISession session,
            IUniqueNameValidator<LabelType> uniqueNameValidator) : base(logger, session)
        {
            _uniqueNameValidator = uniqueNameValidator;
        }

        public async Task Handle(CreateLabelType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.Name))
                throw new NameNotUniqueException();

            var labelType = new LabelType(message.LabelTypeId, message.Name);
            Session.Add(labelType);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateLabelType message)
        {
            if (_uniqueNameValidator.IsNameTaken(message.LabelTypeId, message.Name))
                throw new NameNotUniqueException();

            var labelType = Session.Get<LabelType>(message.LabelTypeId);
            labelType.Update(message.Name);
            await Session.Commit(message.User);
        }
    }
}
