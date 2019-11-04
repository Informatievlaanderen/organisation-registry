namespace OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers
{
    using System;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Domain;
    using Xunit.Abstractions;

    public abstract class ExceptionSpecification<TAggregate, THandler, TCommand> : Specification<TAggregate, THandler, TCommand>
        where TAggregate : AggregateRoot
        where THandler : class, ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        protected Exception Exception { get; set; }

        protected override void HandleEvents()
        {
            try
            {
                base.HandleEvents();
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
        }

        protected ExceptionSpecification(ITestOutputHelper helper) : base(helper) { }
    }
}
