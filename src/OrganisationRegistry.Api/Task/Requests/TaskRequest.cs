namespace OrganisationRegistry.Api.Task.Requests
{
    public class TaskRequest
    {
        public TaskType Type { get; set; }

        public string[] Params { get; set; }
    }
}
