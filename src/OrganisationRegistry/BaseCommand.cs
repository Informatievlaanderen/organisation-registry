namespace OrganisationRegistry
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Infrastructure.Authorization;
    using Infrastructure.Commands;
    using Infrastructure.Messages;

    public class BaseCommand : ICommand
    {
        protected Guid Id { get; set; }

        public int ExpectedVersion { get; set; }

        Guid IMessage.Id
        {
            get => Id;
            set => Id = value;
        }

        public IUser User { get; set; }
    }

    public class BaseCommand<T> : BaseCommand where T : GuidValueObject<T>
    {
        public new T Id { get; set; }
    }
}
