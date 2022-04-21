namespace OrganisationRegistry.Api.Backoffice.Body
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Person;
    using Report.Responses;
    using SqlServer.Infrastructure;
    using SqlServer.Reporting;

    public static class BodyParticipationStatus
    {
        public static bool HasAllSeatsAssigned(OrganisationRegistryContext context, Guid id)
        {
            return !HasActiveBodySeats(context, id) ||
                   !HasNonAssignedActiveBodySeats(context, id);
        }

        public static bool IsMepCompliant(OrganisationRegistryContext context, Guid id)
        {
            var activePostsPerTypeQuery = context
                .BodySeatGenderRatioBodyList
                .Include(item => item.PostsPerType)
                .SingleOrDefault(item => item.BodyId == id)
                ?.PostsPerType
                .Where(post =>
                    (!post.BodySeatValidFrom.HasValue || post.BodySeatValidFrom <= DateTime.Today) &&
                    (!post.BodySeatValidTo.HasValue || post.BodySeatValidTo >= DateTime.Today))
                .Where(post => post.EntitledToVote);

            var bodySeatGenderRatioPostsPerTypeItems = activePostsPerTypeQuery?.ToList() ?? new List<BodySeatGenderRatioPostsPerTypeItem>();
            if (!bodySeatGenderRatioPostsPerTypeItems.Any())
                return true;

            var activePostsPerType =
                bodySeatGenderRatioPostsPerTypeItems
                    .GroupBy(x => new
                    {
                        x.BodyId
                    })
                    .ToList();

            var activeSeatIds = bodySeatGenderRatioPostsPerTypeItems.Select(item => item.BodySeatId);

            var activeMandates = context
                .BodySeatGenderRatioBodyMandateList
                .Include(mandate => mandate.Assignments)
                .Where(mandate => activeSeatIds.Contains(mandate.BodySeatId))
                .Where(mandate =>
                    (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= DateTime.Today) &&
                    (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= DateTime.Today))
                .ToList();

            var activeAssignments =
                activeMandates
                    .SelectMany(mandate => mandate.Assignments)
                    .Where(assignment =>
                        (!assignment.AssignmentValidFrom.HasValue || assignment.AssignmentValidFrom <= DateTime.Today) &&
                        (!assignment.AssignmentValidTo.HasValue || assignment.AssignmentValidTo >= DateTime.Today))
                    .ToList();

            var bodySeatGenderRatioTotals = activePostsPerType
                .Select(g =>
                {
                    var totalCount = g.Count();
                    var assignedCount = activeAssignments.Count;

                    return new BodyParticipationTotals
                    {
                        MaleCount = activeAssignments.Count(x => x.Sex == Sex.Male),
                        FemaleCount = activeAssignments.Count(x => x.Sex == Sex.Female),
                        UnknownCount = activeAssignments.Count(x => !x.Sex.HasValue),

                        AssignedCount = assignedCount,
                        UnassignedCount = totalCount - assignedCount,

                        TotalCount = totalCount
                    };
                }).First();

            if (bodySeatGenderRatioTotals.AssignedCount <= 0)
                return false;

            var lower = Math.Floor((decimal)1 / 3 * 100) / 100;
            var upper = Math.Ceiling((decimal)2 / 3 * 100) / 100;

            var malePercentage = Math.Round((decimal)bodySeatGenderRatioTotals.MaleCount / bodySeatGenderRatioTotals.AssignedCount, 2);
            var femalePercentage = Math.Round((decimal)bodySeatGenderRatioTotals.FemaleCount / bodySeatGenderRatioTotals.AssignedCount, 2);

            var isMepCompliant =
                malePercentage >= lower && malePercentage <= upper &&
                femalePercentage >= lower && femalePercentage <= upper;

            return isMepCompliant;
        }

        private static bool HasActiveBodySeats(OrganisationRegistryContext context, Guid id)
        {
            var hasActiveBodySeats =
                context
                    .BodySeatGenderRatioBodyList
                    .Include(item => item.PostsPerType)
                    .SingleOrDefault(item => item.BodyId == id)
                    ?.PostsPerType
                    .Any(post =>
                        post.BodyId == id &&
                        (!post.BodySeatValidFrom.HasValue || post.BodySeatValidFrom <= DateTime.Today) &&
                        (!post.BodySeatValidTo.HasValue || post.BodySeatValidTo >= DateTime.Today));

            return hasActiveBodySeats ?? false;
        }

        private static bool HasNonAssignedActiveBodySeats(OrganisationRegistryContext context, Guid id)
        {
            //active seats
            var activePostIds =
                context
                    .BodySeatGenderRatioBodyList
                    .Include(item => item.PostsPerType)
                    .Single(item => item.BodyId == id)
                    .PostsPerType
                    .Where(post =>
                        (!post.BodySeatValidFrom.HasValue || post.BodySeatValidFrom <= DateTime.Today) &&
                        (!post.BodySeatValidTo.HasValue || post.BodySeatValidTo >= DateTime.Today))
                    .Select(post => post.BodySeatId);

            //active mandates
            var activeMandates = context
                .BodySeatGenderRatioBodyMandateList
                .Include(mandate => mandate.Assignments)
                .Where(mandate => activePostIds.Contains(mandate.BodySeatId))
                .Where(mandate =>
                    (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= DateTime.Today) &&
                    (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= DateTime.Today));

            //equals?
            if (activePostIds.Count() > activeMandates.Count())
                return true;

            //look for mandates without active assignment
            var hasNonAssignedActiveBodySeats = activeMandates
                .Any(mandate => !mandate.Assignments.Any(assignment =>
                    (!assignment.AssignmentValidFrom.HasValue || assignment.AssignmentValidFrom <= DateTime.Today) &&
                    (!assignment.AssignmentValidTo.HasValue || assignment.AssignmentValidTo >= DateTime.Today)));

            return hasNonAssignedActiveBodySeats;
        }
    }
}
