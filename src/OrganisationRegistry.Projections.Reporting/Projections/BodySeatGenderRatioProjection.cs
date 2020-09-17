namespace OrganisationRegistry.Projections.Reporting.Projections
{
    using Autofac.Features.OwnedInstances;
    using Body.Events;
    using Infrastructure;
    using LifecyclePhaseType;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Organisation.Events;
    using Person.Events;
    using SqlServer.Infrastructure;
    using SqlServer.Reporting;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using OrganisationRegistry.Infrastructure.Events;
    using SeatType.Events;
    using SqlServer;

    public class BodySeatGenderRatioProjection :
        Projection<BodySeatGenderRatioProjection>,

        IEventHandler<OrganisationCreated>,
        IEventHandler<OrganisationCreatedFromKbo>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<OrganisationCouplingWithKboTerminated>,

        IEventHandler<OrganisationBecameActive>,
        IEventHandler<OrganisationBecameInactive>,

        IEventHandler<PersonCreated>,
        IEventHandler<PersonUpdated>,

        IEventHandler<BodyRegistered>,
        IEventHandler<BodyInfoChanged>,

        IEventHandler<BodyLifecyclePhaseAdded>,
        IEventHandler<BodyLifecyclePhaseUpdated>,

        IEventHandler<BodySeatAdded>,
        IEventHandler<BodySeatUpdated>,

        IEventHandler<AssignedPersonToBodySeat>,
        IEventHandler<ReassignedPersonToBodySeat>,

        IEventHandler<AssignedOrganisationToBodySeat>,
        IEventHandler<ReassignedOrganisationToBodySeat>,

        IEventHandler<AssignedFunctionTypeToBodySeat>,
        IEventHandler<ReassignedFunctionTypeToBodySeat>,

        IEventHandler<PersonAssignedToDelegation>,
        IEventHandler<PersonAssignedToDelegationUpdated>,
        IEventHandler<PersonAssignedToDelegationRemoved>,

        IEventHandler<BodyAssignedToOrganisation>,
        IEventHandler<BodyClearedFromOrganisation>,

        IEventHandler<OrganisationOrganisationClassificationAdded>,
        IEventHandler<KboLegalFormOrganisationOrganisationClassificationAdded>,
        IEventHandler<KboLegalFormOrganisationOrganisationClassificationRemoved>,
        IEventHandler<OrganisationOrganisationClassificationUpdated>,

        IEventHandler<SeatTypeUpdated>,

        IEventHandler<InitialiseProjection>
    {
        public BodySeatGenderRatioProjection(
            ILogger<BodySeatGenderRatioProjection> logger,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
        }

        public override string[] ProjectionTableNames =>
            new[]
            {
                BodySeatGenderRatioAssignmentListConfiguration.TableName,
                BodySeatGenderRatioBodyListConfiguration.TableName,
                BodySeatGenderRatioLifecyclePhaseValidityListConfiguration.TableName,
                BodySeatGenderRatioBodyMandateListConfiguration.TableName,
                BodySeatGenderRatioOrganisationClassificationListConfiguration.TableName,
                BodySeatGenderRatioOrganisationListConfiguration.TableName,
                BodySeatGenderRatioOrganisationPerBodyListConfiguration.TableName,
                BodySeatGenderRatioPersonListConfiguration.TableName,
                BodySeatGenderRatioPostsPerTypeListConfiguration.TableName
            };

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreated> message)
        {
            CacheOrganisationName(message.Body.OrganisationId, message.Body.Name);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreatedFromKbo> message)
        {
            CacheOrganisationName(message.Body.OrganisationId, message.Body.Name);
        }

        private void CacheOrganisationName(Guid organisationId, string organisationName)
        {
            using (var context = ContextFactory.Create())
            {
                //organisation cache

                var body = new BodySeatGenderRatioOrganisationListItem
                {
                    OrganisationId = organisationId,
                    OrganisationName = organisationName
                };

                context.BodySeatGenderRatioOrganisationList.Add(body);

                context.SaveChanges();
            }
        }


        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            UpdateOrganisationName(message.Body.OrganisationId, message.Body.Name);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            UpdateOrganisationName(message.Body.OrganisationId, message.Body.Name);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            UpdateOrganisationName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling);

            if (message.Body.LegalFormOrganisationOrganisationClassificationIdToCancel == null)
                return;

            using (var context = ContextFactory.Create())
            {
                var item = context.BodySeatGenderRatioOrganisationClassificationList.Single(x =>
                    x.OrganisationOrganisationClassificationId == message.Body.LegalFormOrganisationOrganisationClassificationIdToCancel);

                context.BodySeatGenderRatioOrganisationClassificationList.Remove(item);

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboTerminated> message)
        {
            if (message.Body.LegalFormOrganisationOrganisationClassificationIdToTerminate == null)
                return;

            using (var context = ContextFactory.Create())
            {
                var legalFormOrganisationClassification = context.BodySeatGenderRatioOrganisationClassificationList.Single(x =>
                    x.OrganisationOrganisationClassificationId == message.Body.LegalFormOrganisationOrganisationClassificationIdToTerminate);

                legalFormOrganisationClassification.ClassificationValidTo = message.Body.DateOfTermination;

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Update organisation cache, organisation per body cache and affected records in projection (organisation name)
        /// </summary>
        private void UpdateOrganisationName(Guid organisationId, string organisationName)
        {
            using (var context = ContextFactory.Create())
            {
                context
                    .BodySeatGenderRatioOrganisationList
                    .Where(item => item.OrganisationId == organisationId)
                    .ToList()
                    .ForEach(item => { item.OrganisationName = organisationName; });

                context
                    .BodySeatGenderRatioOrganisationPerBodyList
                    .Where(item => item.OrganisationId == organisationId)
                    .ToList()
                    .ForEach(item => { item.OrganisationName = organisationName; });

                context
                    .BodySeatGenderRatioBodyList
                    .Where(item => item.OrganisationId == organisationId)
                    .ToList()
                    .ForEach(item => { item.OrganisationName = organisationName; });

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBecameActive> message)
        {
            using (var context = ContextFactory.Create())
            {
                context.BodySeatGenderRatioOrganisationList
                    .Where(item => item.OrganisationId == message.Body.OrganisationId)
                    .ToList()
                    .ForEach(item =>
                    {
                        item.OrganisationActive = true;
                    });

                context.BodySeatGenderRatioOrganisationPerBodyList
                    .Where(item => item.OrganisationId == message.Body.OrganisationId)
                    .ToList()
                    .ForEach(item =>
                    {
                        item.OrganisationActive = true;
                    });

                context.BodySeatGenderRatioBodyList
                    .Where(item => item.OrganisationId == message.Body.OrganisationId)
                    .ToList()
                    .ForEach(item =>
                    {
                        item.OrganisationIsActive = true;
                    });

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBecameInactive> message)
        {
            using (var context = ContextFactory.Create())
            {
                context.BodySeatGenderRatioOrganisationList
                    .Where(item => item.OrganisationId == message.Body.OrganisationId)
                    .ToList()
                    .ForEach(item =>
                    {
                        item.OrganisationActive = false;
                    });

                context.BodySeatGenderRatioOrganisationPerBodyList
                    .Where(item => item.OrganisationId == message.Body.OrganisationId)
                    .ToList()
                    .ForEach(item =>
                    {
                        item.OrganisationActive = false;
                    });

                context.BodySeatGenderRatioBodyList
                    .Where(item => item.OrganisationId == message.Body.OrganisationId)
                    .ToList()
                    .ForEach(item =>
                    {
                        item.OrganisationIsActive = false;
                    });

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Cache person (person id, person fullname, person sex)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonCreated> message)
        {
            using (var context = ContextFactory.Create())
            {
                var person = new BodySeatGenderRatioPersonListItem
                {
                    PersonId = message.Body.PersonId,
                    PersonSex = message.Body.Sex
                };

                context.BodySeatGenderRatioPersonList.Add(person);

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update person cache and affected records in projection (person fullname, person sex)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            using (var context = ContextFactory.Create())
            {
                var person =
                    context
                        .BodySeatGenderRatioPersonList
                        .Single(x => x.PersonId == message.Body.PersonId);

                person.PersonSex = message.Body.Sex;

                context
                    .BodySeatGenderRatioBodyMandateList
                    .Include(mandate => mandate.Assignments)
                    .SelectMany(mandate => mandate.Assignments)
                    .Where(x => x.PersonId == message.Body.PersonId)
                    .ToList()
                    .ForEach(item =>
                    {
                        item.Sex = message.Body.Sex;
                    });

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyRegistered> message)
        {
            using (var context = ContextFactory.Create())
            {
                var organisation = GetOrganisationForBodyFromCache(context, message.Body.BodyId);

                var item = new BodySeatGenderRatioBodyItem
                {
                    BodyId = message.Body.BodyId,
                    BodyName = message.Body.Name,

                    OrganisationId = organisation?.OrganisationId,
                    OrganisationName = organisation?.OrganisationName ?? string.Empty,
                    OrganisationIsActive = organisation?.OrganisationId != null &&
                                           (GetOrganisationFromCache(context, organisation.OrganisationId.Value)
                                                ?.OrganisationActive ?? false),

                    LifecyclePhaseValidities = new List<BodySeatGenderRatioBodyLifecyclePhaseValidityItem>(),
                    PostsPerType = new List<BodySeatGenderRatioPostsPerTypeItem>(),
                };

                context.BodySeatGenderRatioBodyList.Add(item);

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update affected records in projection (body name)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyInfoChanged> message)
        {
            using (var context = ContextFactory.Create())
            {
                context
                    .BodySeatGenderRatioBodyList
                    .Where(x => x.BodyId == message.Body.BodyId)
                    .ToList()
                    .ForEach(item =>
                    {
                        item.BodyName = message.Body.Name;
                    });

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyLifecyclePhaseAdded> message)
        {
            using (var context = ContextFactory.Create())
            {
                var items = context
                    .BodySeatGenderRatioBodyList
                    .Include(item => item.LifecyclePhaseValidities)
                    .Where(item => item.BodyId == message.Body.BodyId)
                    .ToList();

                foreach (var item in items)
                {
                    var lifecycle = item.LifecyclePhaseValidities.SingleOrDefault(x =>
                        x.LifecyclePhaseId == message.Body.BodyLifecyclePhaseId);
                    if (lifecycle != null)
                    {
                        item.LifecyclePhaseValidities.Remove(lifecycle);

                        context.SaveChanges();
                    }

                    var newLifecycle = new BodySeatGenderRatioBodyLifecyclePhaseValidityItem
                    {
                        LifecyclePhaseId = message.Body.BodyLifecyclePhaseId,
                        BodyId = message.Body.BodyId,
                        ValidFrom = message.Body.ValidFrom,
                        ValidTo = message.Body.ValidTo,
                        RepresentsActivePhase = message.Body.LifecyclePhaseTypeIsRepresentativeFor ==
                                                LifecyclePhaseTypeIsRepresentativeFor.ActivePhase
                    };

                    item.LifecyclePhaseValidities.Add(newLifecycle);

                    context.SaveChanges();
                }
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyLifecyclePhaseUpdated> message)
        {
            using (var context = ContextFactory.Create())
            {
                var bodySeatGenderRatioBodyItem = context
                    .BodySeatGenderRatioBodyList
                    .Include(item => item.LifecyclePhaseValidities)
                    .Single(item => item.BodyId == message.Body.BodyId);

                var lifecyclePhaseValidity =
                    bodySeatGenderRatioBodyItem
                    .LifecyclePhaseValidities
                    .Single(validity => validity.LifecyclePhaseId == message.Body.BodyLifecyclePhaseId);

                lifecyclePhaseValidity.ValidFrom = message.Body.ValidFrom;
                lifecyclePhaseValidity.ValidTo = message.Body.ValidTo;
                lifecyclePhaseValidity.RepresentsActivePhase =
                    message.Body.LifecyclePhaseTypeIsRepresentativeFor == LifecyclePhaseTypeIsRepresentativeFor.ActivePhase;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Create new projection record (body, bodyseat, bodyseattype, entitledtovote)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatAdded> message)
        {
            using (var context = ContextFactory.Create())
            {
                var body =
                    context.BodySeatGenderRatioBodyList
                        .Include(x => x.PostsPerType)
                        .Single(bodyItem => bodyItem.BodyId == message.Body.BodyId);

                var item = new BodySeatGenderRatioPostsPerTypeItem()
                {
                    BodyId = message.Body.BodyId,
                    BodySeatId = message.Body.BodySeatId,
                    EntitledToVote = message.Body.EntitledToVote,
                    BodySeatValidFrom = message.Body.ValidFrom,
                    BodySeatValidTo = message.Body.ValidTo,

                    BodySeatTypeId = message.Body.SeatTypeId,
                    BodySeatTypeName = message.Body.SeatTypeName,
                    BodySeatTypeIsEffective = message.Body.SeatTypeIsEffective ?? true,
                };

                body.PostsPerType.Add(item);

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update affected records in projection (bodyseat status, bodyseattype, entitledtovote)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatUpdated> message)
        {
            using (var context = ContextFactory.Create())
            {
                var postsPerType = context
                    .BodySeatGenderRatioBodyList
                    .Include(item => item.PostsPerType)
                    .Single(item => item.BodyId == message.Body.BodyId)
                    .PostsPerType;

                var posts = postsPerType.Where(x => x.BodySeatId == message.Body.BodySeatId);

                foreach (var post in posts)
                {
                    post.EntitledToVote = message.Body.EntitledToVote;
                    post.BodySeatValidFrom = message.Body.ValidFrom;
                    post.BodySeatValidTo = message.Body.ValidTo;

                    post.BodySeatTypeId = message.Body.SeatTypeId;
                    post.BodySeatTypeName = message.Body.SeatTypeName;
                    post.BodySeatTypeIsEffective = message.Body.SeatTypeIsEffective ?? true;

                    context
                        .BodySeatGenderRatioBodyMandateList
                        .Where(mandate => mandate.BodySeatId == message.Body.BodySeatId)
                        .ToList()
                        .ForEach(mandate =>
                        {
                            mandate.BodySeatTypeId = message.Body.SeatTypeId;
                            mandate.BodySeatTypeIsEffective = message.Body.SeatTypeIsEffective ?? true;
                        });
                }

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<SeatTypeUpdated> message)
        {
            using (var context = ContextFactory.Create())
            {
                var bodiesWithThisSeatTypeId = context
                    .BodySeatGenderRatioBodyList
                    .Include(item => item.PostsPerType)
                    .Where(item => item.PostsPerType.Any(post => post.BodySeatTypeId == message.Body.SeatTypeId))
                    .ToList();

                foreach (var body in bodiesWithThisSeatTypeId)
                {
                    foreach (var post in body.PostsPerType.Where(p => p.BodySeatTypeId == message.Body.SeatTypeId))
                    {
                        post.BodySeatTypeName = message.Body.Name;
                        post.BodySeatTypeIsEffective = message.Body.IsEffective ?? true;
                    }
                }

                context
                    .BodySeatGenderRatioBodyMandateList
                    .Where(mandate => mandate.BodySeatTypeId == message.Body.SeatTypeId)
                    .ToList()
                    .ForEach(mandate =>
                    {
                        mandate.BodySeatTypeIsEffective = message.Body.IsEffective ?? true;
                    });

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Update affected records in projection (person id, person fullname, person sex, person assigned status + assigned status + refresh person from cached data)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonToBodySeat> message)
        {
            using (var context = ContextFactory.Create())
            {
                var bodySeatType =
                    context
                        .BodySeatGenderRatioBodyList
                        .Include(item => item.PostsPerType)
                        .Single(item => item.BodyId == message.Body.BodyId)
                        .PostsPerType
                        .First(item => item.BodySeatId == message.Body.BodySeatId);

                var bodyMandate = new BodySeatGenderRatioBodyMandateItem
                {
                    BodyMandateId = message.Body.BodyMandateId,

                    BodyMandateValidFrom = message.Body.ValidFrom,
                    BodyMandateValidTo = message.Body.ValidTo,

                    BodyId = message.Body.BodyId,

                    BodySeatId = message.Body.BodySeatId,

                    BodySeatTypeId = bodySeatType.BodySeatTypeId,

                    BodySeatTypeIsEffective = bodySeatType.BodySeatTypeIsEffective,

                    Assignments = new List<BodySeatGenderRatioAssignmentItem>()
                };

                var personFromCache = GetPersonFromCache(context, message.Body.PersonId);
                var assignment = new BodySeatGenderRatioAssignmentItem
                {
                    BodyMandateId = message.Body.BodyMandateId,

                    DelegationAssignmentId = null,

                    AssignmentValidFrom = message.Body.ValidFrom,
                    AssignmentValidTo = message.Body.ValidTo,

                    PersonId = message.Body.PersonId,
                    Sex = personFromCache.Sex
                };

                bodyMandate.Assignments.Add(assignment);

                context.BodySeatGenderRatioBodyMandateList.Add(bodyMandate);

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update affected records in projection (person id, person fullname, person sex, person assigned statusassigned status +  + refresh person from cached data)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedPersonToBodySeat> message)
        {
            //called on update mandate
            using (var context = ContextFactory.Create())
            {
                var bodyMandate =
                    context
                        .BodySeatGenderRatioBodyMandateList
                        .Include(mandate => mandate.Assignments)
                        .Single(x => x.BodyMandateId == message.Body.BodyMandateId);

                if (!message.Body.BodySeatId.Equals(message.Body.PreviousBodySeatId))
                {
                    var bodySeatType =
                        context
                            .BodySeatGenderRatioBodyList
                            .Include(item => item.PostsPerType)
                            .Single(item => item.BodyId == message.Body.BodyId)
                            .PostsPerType
                            .First(item => item.BodySeatId == message.Body.BodySeatId);

                    bodyMandate.BodySeatId = message.Body.BodySeatId;
                    bodyMandate.BodySeatTypeId = bodySeatType.BodySeatTypeId;
                    bodyMandate.BodySeatTypeIsEffective = bodySeatType.BodySeatTypeIsEffective;
                }

                bodyMandate.BodyMandateValidFrom = message.Body.ValidFrom;
                bodyMandate.BodyMandateValidTo = message.Body.ValidTo;

                var assignment = bodyMandate.Assignments.First();
                assignment.AssignmentValidFrom = message.Body.ValidFrom;
                assignment.AssignmentValidTo = message.Body.ValidTo;
                assignment.PersonId = message.Body.PersonId;
                assignment.Sex = GetPersonFromCache(context, message.Body.PersonId).Sex;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update affected records in projection (organisation assigned status + assigned status + refresh organisation from cached data)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedOrganisationToBodySeat> message)
        {
            using (var context = ContextFactory.Create())
            {
                var bodySeatType =
                    context
                        .BodySeatGenderRatioBodyList
                        .Include(item => item.PostsPerType)
                        .Single(item => item.BodyId == message.Body.BodyId)
                        .PostsPerType
                        .First(item => item.BodySeatId == message.Body.BodySeatId);

                var bodyMandate = new BodySeatGenderRatioBodyMandateItem
                {
                    BodyMandateId = message.Body.BodyMandateId,

                    BodyMandateValidFrom = message.Body.ValidFrom,
                    BodyMandateValidTo = message.Body.ValidTo,

                    BodyId = message.Body.BodyId,

                    BodySeatId = message.Body.BodySeatId,

                    BodySeatTypeId = bodySeatType.BodySeatTypeId,

                    BodySeatTypeIsEffective = bodySeatType.BodySeatTypeIsEffective,

                    Assignments = new List<BodySeatGenderRatioAssignmentItem>()
                };

                context.BodySeatGenderRatioBodyMandateList.Add(bodyMandate);

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update affected records in projection (organisation assigned status + assigned status + refresh organisation from cached data)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedOrganisationToBodySeat> message)
        {
            using (var context = ContextFactory.Create())
            {
                var item =
                    context
                        .BodySeatGenderRatioBodyMandateList
                        .Single(x => x.BodyMandateId == message.Body.BodyMandateId);

                if (!message.Body.BodySeatId.Equals(message.Body.PreviousBodySeatId))
                {
                    var bodySeatType =
                        context
                            .BodySeatGenderRatioBodyList
                            .Include(x => x.PostsPerType)
                            .Single(x => x.BodyId == message.Body.BodyId)
                            .PostsPerType
                            .First(x => x.BodySeatId == message.Body.BodySeatId);

                    item.BodySeatId = message.Body.BodySeatId;
                    item.BodySeatTypeId = bodySeatType.BodySeatTypeId;
                    item.BodySeatTypeIsEffective = bodySeatType.BodySeatTypeIsEffective;
                }

                item.BodyMandateValidFrom = message.Body.ValidFrom;
                item.BodyMandateValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update affected records in projection (function assigned status + assigned status + refresh organisation from cached data)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedFunctionTypeToBodySeat> message)
        {
            using (var context = ContextFactory.Create())
            {
                var bodySeatType =
                    context
                        .BodySeatGenderRatioBodyList
                        .Include(item => item.PostsPerType)
                        .Single(item => item.BodyId == message.Body.BodyId)
                        .PostsPerType
                        .First(item => item.BodySeatId == message.Body.BodySeatId);

                var bodyMandate = new BodySeatGenderRatioBodyMandateItem
                {
                    BodyMandateId = message.Body.BodyMandateId,

                    BodyMandateValidFrom = message.Body.ValidFrom,
                    BodyMandateValidTo = message.Body.ValidTo,

                    BodyId = message.Body.BodyId,

                    BodySeatId = message.Body.BodySeatId,
                    BodySeatTypeId = bodySeatType.BodySeatTypeId,
                    BodySeatTypeIsEffective = bodySeatType.BodySeatTypeIsEffective,

                    Assignments = new List<BodySeatGenderRatioAssignmentItem>()
                };

                context.BodySeatGenderRatioBodyMandateList.Add(bodyMandate);

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update affected records in projection (function assigned status + assigned status + refresh organisation from cached data)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedFunctionTypeToBodySeat> message)
        {
            using (var context = ContextFactory.Create())
            {
                var item =
                    context
                        .BodySeatGenderRatioBodyMandateList
                        .Single(x => x.BodyMandateId == message.Body.BodyMandateId);

                if (!message.Body.BodySeatId.Equals(message.Body.PreviousBodySeatId))
                {
                    var bodySeatType =
                        context
                            .BodySeatGenderRatioBodyList
                            .Include(x => x.PostsPerType)
                            .Single(x => x.BodyId == message.Body.BodyId)
                            .PostsPerType
                            .First(x => x.BodySeatId == message.Body.BodySeatId);

                    item.BodySeatId = message.Body.BodySeatId;
                    item.BodySeatTypeId = bodySeatType.BodySeatTypeId;
                    item.BodySeatTypeIsEffective = bodySeatType.BodySeatTypeIsEffective;
                }

                item.BodyMandateValidFrom = message.Body.ValidFrom;
                item.BodyMandateValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update affected records in projection (person id, person fullname, person sex, person assigned status + assigned status)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegation> message)
        {
            using (var context = ContextFactory.Create())
            {
                var bodyMandate =
                    context
                        .BodySeatGenderRatioBodyMandateList
                        .Include(mandate => mandate.Assignments)
                        .Single(item => item.BodyMandateId == message.Body.BodyMandateId);

                var personFromCache = GetPersonFromCache(context, message.Body.PersonId);
                bodyMandate.Assignments.Add(new BodySeatGenderRatioAssignmentItem
                {
                    BodyMandateId = message.Body.BodyMandateId,

                    DelegationAssignmentId = message.Body.DelegationAssignmentId,

                    AssignmentValidFrom = message.Body.ValidFrom,
                    AssignmentValidTo = message.Body.ValidTo,

                    PersonId = message.Body.PersonId,
                    Sex = personFromCache.Sex
                });

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update affected records in projection (person id, person fullname, person sex, person assigned status + assigned status)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationUpdated> message)
        {
            using (var context = ContextFactory.Create())
            {
                var bodyMandate =
                    context
                        .BodySeatGenderRatioBodyMandateList
                        .Include(mandate => mandate.Assignments)
                        .Single(item => item.BodyMandateId == message.Body.BodyMandateId);

                var assignment =
                    bodyMandate
                        .Assignments
                        .Single(item => item.DelegationAssignmentId == message.Body.DelegationAssignmentId);

                assignment.AssignmentValidFrom = message.Body.ValidFrom;
                assignment.AssignmentValidTo = message.Body.ValidTo;

                assignment.PersonId = message.Body.PersonId;
                assignment.Sex = GetPersonFromCache(context, message.Body.PersonId).Sex;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update affected records in projection (person assigned status + assigned status)
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationRemoved> message)
        {
            using (var context = ContextFactory.Create())
            {
                var item = context
                    .BodySeatGenderRatioBodyMandateList
                    .Include(mandate => mandate.Assignments)
                    .Single(x =>
                        x.BodyMandateId == message.Body.BodyMandateId);

                var assignment = item
                    .Assignments
                    .Single(assignmentItem =>
                        assignmentItem.DelegationAssignmentId == message.Body.DelegationAssignmentId);

                item.Assignments.Remove(assignment);

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update affected records in projection (organisation id, organisation name, organisation assigned status) + assigned status
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyAssignedToOrganisation> message)
        {
            using (var context = ContextFactory.Create())
            {
                var cachedOrganisation = GetOrganisationFromCache(context, message.Body.OrganisationId);

                //organisation per body cache
                var body = new BodySeatGenderRatioOrganisationPerBodyListItem
                {
                    BodyId = message.Body.BodyId,
                    BodyOrganisationId = message.Body.BodyOrganisationId,
                    OrganisationId = message.Body.OrganisationId,
                    OrganisationName = message.Body.OrganisationName,
                    OrganisationActive = cachedOrganisation?.OrganisationActive ?? false
                };

                context.BodySeatGenderRatioOrganisationPerBodyList.Add(body);

                context.BodySeatGenderRatioBodyList
                    .Where(post => post.BodyId == message.Body.BodyId)
                    .ToList()
                    .ForEach(post =>
                    {
                        post.OrganisationId = message.Body.OrganisationId;
                        post.OrganisationName = message.Body.OrganisationName;
                        post.OrganisationIsActive = cachedOrganisation?.OrganisationActive ?? false;
                    });

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Update affected records in projection organisation assigned status + assigned status
        /// </summary>
        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClearedFromOrganisation> message)
        {
            using (var context = ContextFactory.Create())
            {
                var body = context
                    .BodySeatGenderRatioOrganisationPerBodyList
                    .SingleOrDefault(x => x.BodyId == message.Body.BodyId);

                if (body == null)
                    return;

                context.BodySeatGenderRatioOrganisationPerBodyList.Remove(body);

                context.BodySeatGenderRatioBodyList
                    .Where(post => post.BodyId == message.Body.BodyId)
                    .ToList()
                    .ForEach(post =>
                    {
                        post.OrganisationId = null;
                        post.OrganisationName = string.Empty;
                        post.OrganisationIsActive = false;
                    });

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationAdded> message)
        {
            AddOrganisationClassification(message.Body.OrganisationOrganisationClassificationId, message.Body.OrganisationId, message.Body.OrganisationClassificationId, message.Body.OrganisationClassificationTypeId, message.Body.ValidFrom, message.Body.ValidTo);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationAdded> message)
        {
            AddOrganisationClassification(message.Body.OrganisationOrganisationClassificationId, message.Body.OrganisationId, message.Body.OrganisationClassificationId, message.Body.OrganisationClassificationTypeId, message.Body.ValidFrom, message.Body.ValidTo);
        }

        private void AddOrganisationClassification(Guid organisationOrganisationClassificationId, Guid organisationId, Guid organisationClassificationId, Guid organisationClassificationTypeId, DateTime? validFrom, DateTime? validTo)
        {
            using (var context = ContextFactory.Create())
            {
                context.BodySeatGenderRatioOrganisationClassificationList.Add(
                    new BodySeatGenderRatioOrganisationClassificationItem
                    {
                        OrganisationOrganisationClassificationId = organisationOrganisationClassificationId,

                        OrganisationId = organisationId,
                        OrganisationClassificationId = organisationClassificationId,
                        OrganisationClassificationTypeId = organisationClassificationTypeId,

                        ClassificationValidFrom = validFrom,
                        ClassificationValidTo = validTo
                    });

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationRemoved> message)
        {
            using (var context = ContextFactory.Create())
            {
                var item = context.BodySeatGenderRatioOrganisationClassificationList.Single(x =>
                    x.OrganisationOrganisationClassificationId == message.Body.OrganisationOrganisationClassificationId);

                context.BodySeatGenderRatioOrganisationClassificationList.Remove(item);

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationUpdated> message)
        {
            using (var context = ContextFactory.Create())
            {
                var item = context.BodySeatGenderRatioOrganisationClassificationList.Single(x =>
                    x.OrganisationOrganisationClassificationId == message.Body.OrganisationOrganisationClassificationId);

                item.OrganisationId = message.Body.OrganisationId;
                item.OrganisationClassificationId = message.Body.OrganisationClassificationId;
                item.OrganisationClassificationTypeId = message.Body.OrganisationClassificationTypeId;

                item.ClassificationValidFrom = message.Body.ValidFrom;
                item.ClassificationValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<InitialiseProjection> message)
        {
            if (message.Body.ProjectionName != typeof(BodySeatGenderRatioProjection).FullName)
                return;

            Logger.LogInformation("Clearing tables for {ProjectionName}.", message.Body.ProjectionName);

            using (var context = ContextFactory.Create())
                context.Database.ExecuteSqlCommand(
                    string.Concat(ProjectionTableNames.Select(tableName => $"DELETE FROM [OrganisationRegistry].[{tableName}];")));
        }

        private static CachedPerson GetPersonFromCache(OrganisationRegistryContext context, Guid personId)
        {
            var person = context
                .BodySeatGenderRatioPersonList
                .SingleOrDefault(x => x.PersonId == personId);

            return person != null
                ? CachedPerson.FromCache(person)
                : CachedPerson.Empty();
        }

        private static CachedOrganisation GetOrganisationFromCache(OrganisationRegistryContext context, Guid organisationId)
        {
            var organisation = context
                .BodySeatGenderRatioOrganisationList
                .SingleOrDefault(x => x.OrganisationId == organisationId);

            return organisation != null
                ? CachedOrganisation.FromCache(organisation)
                : CachedOrganisation.Empty();
        }

        private static CachedOrganisationForBody GetOrganisationForBodyFromCache(OrganisationRegistryContext context, Guid bodyId)
        {
            var organisationPerBody = context
                .BodySeatGenderRatioOrganisationPerBodyList
                .SingleOrDefault(x => x.BodyId == bodyId);

            return organisationPerBody != null
                ? CachedOrganisationForBody.FromCache(organisationPerBody)
                : CachedOrganisationForBody.Empty();
        }
    }
}
