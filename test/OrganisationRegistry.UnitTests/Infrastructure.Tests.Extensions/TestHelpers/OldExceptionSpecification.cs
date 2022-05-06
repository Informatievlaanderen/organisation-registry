namespace OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers
{
    using System;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Domain;
    using Xunit.Abstractions;

    public abstract class OldExceptionSpecification<TAggregate, THandler, TCommand> : OldSpecification<TAggregate, THandler, TCommand>
        where TAggregate : AggregateRoot
        where THandler : class, ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        protected Exception Exception { get; set; }

        protected override async Task HandleEvents()
        {
            try
            {
                await base.HandleEvents();
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
        }

        protected OldExceptionSpecification(ITestOutputHelper helper) : base(helper) { }
    }
}
