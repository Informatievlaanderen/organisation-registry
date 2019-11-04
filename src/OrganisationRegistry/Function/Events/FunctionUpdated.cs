namespace OrganisationRegistry.Function.Events
{
    using System;

    public class FunctionUpdated : BaseEvent<FunctionUpdated>
    {
        public Guid FunctionId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public FunctionUpdated(
            Guid functionId,
            string name,
            string previousName)
        {
            Id = functionId;

            Name = name;
            PreviousName = previousName;
        }
    }
}
