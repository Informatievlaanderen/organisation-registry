namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Body;
using FormalFramework;
using Microsoft.EntityFrameworkCore;
using Organisation;
using SqlServer.Body.ScheduledActions.Organisation;
using SqlServer.Organisation;
using SqlServer.Organisation.ScheduledActions.FormalFramework;
using SqlServer.Organisation.ScheduledActions.Parent;
using ActiveBodyMandateAssignment =
    SqlServer.Body.ScheduledActions.PeopleAssignedToBodyMandates.ActivePeopleAssignedToBodyMandateListItem;
using FutureBodyMandateAssignment =
    SqlServer.Body.ScheduledActions.PeopleAssignedToBodyMandates.FuturePeopleAssignedToBodyMandatesListItem;

/// <summary>
/// had to use some magic in order to get this to compile:
/// https://docs.microsoft.com/en-us/ef/core/miscellaneous/async
/// tldr; that's where AsQueryable() and AsAsyncEnumerable() calls come in ...
/// </summary>
public static class ScheduledCommands
{
    public static async Task<List<UpdateCurrentOrganisationParent>> GetScheduledCommandsToExecute(
        this DbSet<ActiveOrganisationParentListItem> list, DateTime date) =>
        await list.AsQueryable()
            .Where(item => item.ValidTo.HasValue && item.ValidTo.Value < date)
            .Select(item => new UpdateCurrentOrganisationParent(new OrganisationId(item.OrganisationId)))
            .AsAsyncEnumerable()
            .ToListAsync();

    public static async Task<List<UpdateCurrentOrganisationParent>> GetScheduledCommandsToExecute(
        this DbSet<FutureActiveOrganisationParentListItem> list, DateTime date) =>
        await list.AsQueryable()
            .Where(item => item.ValidFrom.HasValue && item.ValidFrom.Value <= date)
            .Select(item => new UpdateCurrentOrganisationParent(new OrganisationId(item.OrganisationId)))
            .AsAsyncEnumerable()
            .ToListAsync();

    public static async Task<List<UpdateCurrentBodyOrganisation>> GetScheduledCommandsToExecute(
        this DbSet<ActiveBodyOrganisationListItem> list, DateTime date) =>
        await list.AsQueryable()
            .Where(item => item.ValidTo.HasValue && item.ValidTo.Value < date)
            .Select(item => new UpdateCurrentBodyOrganisation(new BodyId(item.BodyId)))
            .AsAsyncEnumerable()
            .ToListAsync();

    public static async Task<List<UpdateCurrentBodyOrganisation>> GetScheduledCommandsToExecute(
        this DbSet<FutureActiveBodyOrganisationListItem> list, DateTime date) =>
        await list.AsQueryable()
            .Where(item => item.ValidFrom.HasValue && item.ValidFrom.Value <= date)
            .Select(item => new UpdateCurrentBodyOrganisation(new BodyId(item.BodyId)))
            .AsAsyncEnumerable()
            .ToListAsync();

    public static Task<List<UpdateCurrentPersonAssignedToBodyMandate>> GetScheduledCommandsToExecute(
        this DbSet<ActiveBodyMandateAssignment> list, DateTime date) =>
        Task.FromResult(MandateAssignmentsThatHaveBecomeInvalid(list, date)
            .Select(group =>
                new UpdateCurrentPersonAssignedToBodyMandate(
                    new BodyId(group.Key),
                    group.Value.Select(item =>
                        (new BodySeatId(item.BodySeatId), new BodyMandateId(item.BodyMandateId))).ToList()))
            .ToList());

    private static Dictionary<Guid, IEnumerable<ActiveBodyMandateAssignment>>
        MandateAssignmentsThatHaveBecomeInvalid(DbSet<ActiveBodyMandateAssignment> list, DateTime date) =>
        list.AsQueryable()
            .Where(item => item.ValidTo.HasValue && item.ValidTo.Value < date)
            .AsEnumerable()
            .GroupBy(item => item.BodyId)
            .ToDictionary(group => group.Key, group => group.AsEnumerable());

    public static Task<List<UpdateCurrentPersonAssignedToBodyMandate>> GetScheduledCommandsToExecute(this DbSet<FutureBodyMandateAssignment> list,
        DateTime date) =>
        Task.FromResult(MandateAssignmentsThatHaveBecomeValid(list, date)
            .Select(group =>
                new UpdateCurrentPersonAssignedToBodyMandate(
                    new BodyId(group.Key),
                    group.Value.Select(item =>
                        (new BodySeatId(item.BodySeatId), new BodyMandateId(item.BodyMandateId))).ToList()))
            .ToList());

    private static Dictionary<Guid, IEnumerable<FutureBodyMandateAssignment>> MandateAssignmentsThatHaveBecomeValid(
        DbSet<FutureBodyMandateAssignment> list, DateTime date) =>
        list.AsQueryable()
            .Where(item => item.ValidFrom.HasValue && item.ValidFrom.Value <= date)
            .AsEnumerable()
            .GroupBy(item => item.BodyId)
            .ToDictionary(group => group.Key, group => group.AsEnumerable());

    public static async Task<List<UpdateOrganisationFormalFrameworkParents>> GetScheduledCommandsToExecute(
        this DbSet<ActiveOrganisationFormalFrameworkListItem> list, DateTime date) =>
        await list.AsAsyncEnumerable()
            .Where(item => item.ValidTo.HasValue && item.ValidTo.Value < date)
            .Select(item =>
                new UpdateOrganisationFormalFrameworkParents(new OrganisationId(item.OrganisationId),
                    new FormalFrameworkId(item.FormalFrameworkId)))
            .ToListAsync();

    public static async Task<List<UpdateOrganisationFormalFrameworkParents>> GetScheduledCommandsToExecute(
        this DbSet<FutureActiveOrganisationFormalFrameworkListItem> list, DateTime date) =>
        await list.AsAsyncEnumerable()
            .Where(item => item.ValidFrom.HasValue && item.ValidFrom.Value <= date)
            .Select(item =>
                new UpdateOrganisationFormalFrameworkParents(new OrganisationId(item.OrganisationId),
                    new FormalFrameworkId(item.FormalFrameworkId)))
            .ToListAsync();

    public static async Task<List<UpdateRelationshipValidities>> GetScheduledCommandsToExecute(
        this DbSet<OrganisationCapacityListItem> list, DateTime date)
    {
        var shouldBeActive =
            list.AsQueryable()
                .Where(item => !item.IsActive &&
                               (item.ValidFrom == null || item.ValidFrom <= date) &&
                               (item.ValidTo == null || item.ValidTo >= date))
                .Select(item => item.OrganisationId)
                .Distinct();

        var shouldBeInactive =
            list.AsQueryable()
                .Where(item => item.IsActive &&
                               (item.ValidFrom != null && item.ValidFrom > date ||
                                item.ValidTo != null && item.ValidTo < date))
                .Select(item => item.OrganisationId)
                .Distinct();

        return await shouldBeActive
            .Union(shouldBeInactive)
            .Distinct()
            .Select(id => new UpdateRelationshipValidities(new OrganisationId(id), date))
            .ToListAsync();
    }
}
