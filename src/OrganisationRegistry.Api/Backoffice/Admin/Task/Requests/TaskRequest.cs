namespace OrganisationRegistry.Api.Backoffice.Admin.Task.Requests
{
    public class TaskRequest
    {
        public TaskType Type { get; set; }

        public string[] Params { get; set; }
    }
}
