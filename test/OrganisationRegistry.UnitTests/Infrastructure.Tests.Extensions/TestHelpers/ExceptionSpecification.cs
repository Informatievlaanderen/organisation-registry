namespace OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;

using System;
using System.Threading.Tasks;
using OrganisationRegistry.Infrastructure.Commands;
using Xunit.Abstractions;

public abstract class ExceptionSpecification<THandler, TCommand> : Specification<THandler, TCommand>
    where THandler : class, ICommandEnvelopeHandler<TCommand>
    where TCommand : ICommand
{
    protected Exception? Exception { get; private set; }

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

    protected ExceptionSpecification(ITestOutputHelper helper) : base(helper) { }
}
