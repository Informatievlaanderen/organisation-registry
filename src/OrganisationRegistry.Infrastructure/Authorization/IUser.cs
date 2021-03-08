namespace OrganisationRegistry.Infrastructure.Authorization
{
    public interface IUser
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string UserId { get; set; }
        string? Ip { get; set; }
        Role[] Roles { get; set; }
        bool IsInRole(Role role);
    }
}
