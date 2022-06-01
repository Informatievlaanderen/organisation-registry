namespace OrganisationRegistry.Api.Backoffice.Admin.Task.Requests;

using System;

public class TaskRequest
{
    public TaskType Type { get; set; }

    public string[] Params { get; set; } = Array.Empty<string>();
}
