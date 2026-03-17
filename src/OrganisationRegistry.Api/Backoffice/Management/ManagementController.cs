namespace OrganisationRegistry.Api.Backoffice.Management;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("mgmt")]
public class ManagementController : OrganisationRegistryController
{
    /// <summary>Get a list of day names for the current culture.</summary>
    /// <response code="200">Returns a list of days of the week with their localized names.</response>
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

    /// <summary>Get a list of available time slots in 30-minute intervals.</summary>
    /// <response code="200">Returns a list of hours from 00:00 to 23:30 in 30-minute increments.</response>
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
