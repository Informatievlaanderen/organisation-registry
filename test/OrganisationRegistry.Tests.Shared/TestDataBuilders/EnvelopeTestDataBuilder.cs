namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using Infrastructure.Events;

    public class EnvelopeTestDataBuilder
    {
        private int _number;

        public EnvelopeTestDataBuilder()
        {
            _number = 1;
        }

        public EnvelopeTestDataBuilder WithNumber(int number)
        {
            _number = number;
            return this;
        }

        public IEnvelope Build()
            => new Envelope<IEvent<IEvent>>(
                id: Guid.NewGuid(),
                version: 1,
                name: "",
                timestamp: DateTimeOffset.Now,
                body: null,
                ip: "",
                lastName: "",
                firstName: "",
                userId: "")
            {
                Number = _number
            };

        public static implicit operator Envelope(EnvelopeTestDataBuilder builder)
            => (Envelope)builder.Build();
    }
}
