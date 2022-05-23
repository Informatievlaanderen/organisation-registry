namespace OrganisationRegistry.LifecyclePhaseType
{
    using Events;
    using Infrastructure.Domain;

    public class LifecyclePhaseType : AggregateRoot
    {
        public LifecyclePhaseTypeName Name { get; private set; }
        public LifecyclePhaseTypeIsRepresentativeFor LifecyclePhaseTypeIsRepresentativeFor { get; private set; }
        public LifecyclePhaseTypeStatus Status { get; private set; }

        public bool IsForActivePhase => LifecyclePhaseTypeIsRepresentativeFor ==
                                        LifecyclePhaseTypeIsRepresentativeFor.ActivePhase;

        public bool IsForInactivePhase => LifecyclePhaseTypeIsRepresentativeFor ==
                                        LifecyclePhaseTypeIsRepresentativeFor.InactivePhase;

        private LifecyclePhaseType()
        {
            Name = new LifecyclePhaseTypeName(string.Empty);
        }

        public LifecyclePhaseType(
            LifecyclePhaseTypeId id,
            LifecyclePhaseTypeName name,
            LifecyclePhaseTypeIsRepresentativeFor lifecyclePhaseTypeIsRepresentativeFor,
            LifecyclePhaseTypeStatus status)
        {
            Name = new LifecyclePhaseTypeName(string.Empty);

            ApplyChange(new LifecyclePhaseTypeCreated(id, name, lifecyclePhaseTypeIsRepresentativeFor, status));
        }

        public void Update(
            LifecyclePhaseTypeName name,
            LifecyclePhaseTypeIsRepresentativeFor lifecyclePhaseTypeIsRepresentativeFor,
            LifecyclePhaseTypeStatus status)
        {
            var @event = new LifecyclePhaseTypeUpdated(
                new LifecyclePhaseTypeId(Id),
                name, Name,
                lifecyclePhaseTypeIsRepresentativeFor, LifecyclePhaseTypeIsRepresentativeFor,
                status, Status);

            ApplyChange(@event);
        }

        private void Apply(LifecyclePhaseTypeCreated @event)
        {
            Id = @event.LifecyclePhaseTypeId;
            Name = new LifecyclePhaseTypeName(@event.Name);
            LifecyclePhaseTypeIsRepresentativeFor = @event.LifecyclePhaseTypeIsRepresentativeFor;
            Status = @event.Status;
        }

        private void Apply(LifecyclePhaseTypeUpdated @event)
        {
            Name = new LifecyclePhaseTypeName(@event.Name);
            LifecyclePhaseTypeIsRepresentativeFor = @event.LifecyclePhaseTypeIsRepresentativeFor;
            Status = @event.Status;
        }
    }
}
