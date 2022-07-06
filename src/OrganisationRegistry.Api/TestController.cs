namespace OrganisationRegistry.Api;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("test")]
public class TestController : OrganisationRegistryController
{
    public TestController(ICommandSender commandSender) : base(commandSender)
    {
    }

    [HttpGet]
    public IActionResult Test()
        => Ok(GetDataAsync());

    private static IEnumerable<TestResponseItem> GetDataAsync()
    {
        foreach (var index in Enumerable.Range(start: 0, count: 100))
        {
            Thread.Sleep(1000);
            yield return new TestResponseItem(index, Guid.NewGuid());
        }
    }
}

public record TestResponseItem(int Index, Guid Guid);
