namespace OrganisationRegistry.Function.Events
{
    using System;

    public class FunctionCreated : BaseEvent<FunctionCreated>
    {
        public Guid FunctionId => Id;

        public string Name { get; }

        public FunctionCreated(
            Guid functionId,
            string name)
        {
            Id = functionId;
            Name = name;
        }
    }
}
