namespace OrganisationRegistry.Api.Backoffice.Management;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("mgmt")]
public class ManagementController : OrganisationRegistryController
{
    public ManagementController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    [HttpGet("days")]
    public async Task<IActionResult> GetDayNames()
    {
        var days = new List<Day>();

        foreach (var dayOfWeek in ((DayOfWeek[])Enum.GetValues(typeof(DayOfWeek))).ToList())
        {
            days.Add(
                new Day(
                    dayOfWeek,
                    Thread.CurrentThread.CurrentCulture.DateTimeFormat.GetDayName(dayOfWeek)
                ));
        }

        return await OkAsync(days);
    }

    [HttpGet("hours")]
    public async Task<IActionResult> GetHours()
    {
        var hours = new List<Hour>();

        for (var ts = TimeSpan.Zero;
             ts <= TimeSpan.Zero.Add(new TimeSpan(23, 30, 0));
             ts = ts.Add(new TimeSpan(0, 30, 0)))
            hours.Add(new Hour(ts, $"{ts.Hours:00}:{ts.Minutes:00}"));

        return await OkAsync(hours);
    }
}

public record Day(DayOfWeek DayOfWeek, string Label);

public record Hour(TimeSpan TimeSpan, string Label);