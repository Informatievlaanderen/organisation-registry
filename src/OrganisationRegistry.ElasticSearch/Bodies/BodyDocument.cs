namespace OrganisationRegistry.ElasticSearch.Bodies;

using Common;
using Osc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class BodyDocument : IDocument
{
    public BodyDocument()
    {
        Seats = new List<BodySeat>();
        LifecyclePhases = new List<BodyLifecyclePhase>();
        Organisations = new List<BodyOrganisation>();
        FormalFrameworks = new List<BodyFormalFramework>();
        Participations = new List<BodyParticipation>();
        Classifications = new List<BodyClassification>();
        FormalValidity = Period.Infinite();
    }

    public int ChangeId { get; set; }

    public DateTimeOffset ChangeTime { get; set; }

    public Guid Id { get; set; }

    public string BodyNumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? ShortName { get; set; }

    public string? Description { get; set; }

    public Period FormalValidity { get; set; }

    public IList<BodySeat> Seats { get; set; }

    public IList<BodyLifecyclePhase> LifecyclePhases { get; set; }

    public IList<BodyOrganisation> Organisations { get; set; }

    public IList<BodyFormalFramework> FormalFrameworks { get; set; }

    public IList<BodyParticipation> Participations { get; set; }

    public IList<BodyClassification> Classifications { get; set; }

    public static TypeMappingDescriptor<BodyDocument> Mapping(TypeMappingDescriptor<BodyDocument> map)
        => map
            .Properties(
                props => props
                    .Number(
                        n => n
                            .Name(p => p.ChangeId))
                    .Date(
                        d => d
                            .Name(p => p.ChangeTime))
                    .Keyword(
                        k => k
                            .Name(p => p.Id))
                    .Keyword(
                        k => k
                            .Name(p => p.BodyNumber))
                    .Text(
                        t => t
                            .Name(p => p.Name)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Text(
                        t => t
                            .Name(p => p.ShortName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Text(
                        t => t
                            .Name(p => p.Description)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Object<Period>(
                        o => o
                            .Name(p => p.FormalValidity)
                            .Properties(Period.Mapping))
                    .Nested<BodySeat>(
                        n => n
                            .Name(p => p.Seats)
                            .IncludeInRoot()
                            .Properties(BodySeat.Mapping))
                    .Nested<BodyLifecyclePhase>(
                        n => n
                            .Name(p => p.LifecyclePhases)
                            .IncludeInRoot()
                            .Properties(BodyLifecyclePhase.Mapping))
                    .Nested<BodyOrganisation>(
                        n => n
                            .Name(p => p.Organisations)
                            .IncludeInRoot()
                            .Properties(BodyOrganisation.Mapping))
                    .Nested<BodyFormalFramework>(
                        n => n
                            .Name(p => p.FormalFrameworks)
                            .IncludeInRoot()
                            .Properties(BodyFormalFramework.Mapping))
                    .Nested<BodyParticipation>(
                        n => n
                            .Name(p => p.Participations)
                            .IncludeInRoot()
                            .Properties(BodyParticipation.Mapping))
                    .Nested<BodyClassification>(
                        n => n
                            .Name(p => p.Classifications)
                            .IncludeInRoot()
                            .Properties(BodyClassification.Mapping))
            );

    public class BodySeat
    {
        public Guid BodySeatId { get; set; }

        public string BodySeatNumber { get; set; }

        public string Name { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public bool PaidSeat { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public bool EntitledToVote { get; set; }

        public Guid SeatTypeId { get; set; }

        public string SeatTypeName { get; set; }

        public Period Validity { get; set; }

        public IList<BodyMandate> Mandates { get; set; }

        public BodySeat(
            Guid bodySeatId,
            string bodySeatNumber,
            string name,
            bool paidSeat,
            bool entitledToVote,
            Guid seatTypeId,
            string seatTypeName,
            Period validity)
        {
            Mandates = new List<BodyMandate>();

            BodySeatId = bodySeatId;
            BodySeatNumber = bodySeatNumber;
            Name = name;
            PaidSeat = paidSeat;
            EntitledToVote = entitledToVote;
            SeatTypeId = seatTypeId;
            SeatTypeName = seatTypeName;
            Validity = validity;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<BodySeat> map)
            => map
                .Keyword(
                    k => k
                        .Name(p => p.BodySeatId))
                .Keyword(
                    k => k
                        .Name(p => p.BodySeatNumber))
                .Text(
                    t => t
                        .Name(p => p.Name)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Boolean(
                    b => b
                        .Name(p => p.PaidSeat))
                .Boolean(
                    b => b
                        .Name(p => p.EntitledToVote))
                .Keyword(
                    k => k
                        .Name(p => p.SeatTypeId))
                .Text(
                    t => t
                        .Name(p => p.SeatTypeName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Nested<BodyMandate>(
                    n => n
                        .Name(p => p.Mandates)
                        .IncludeInRoot()
                        .Properties(BodyMandate.Mapping))
                .Object<Period>(
                    o => o
                        .Name(p => p.Validity)
                        .Properties(Period.Mapping));
    }

    public class BodyMandate
    {
        public Guid BodyMandateId { get; set; }

        public Guid? OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public Guid? FunctionTypeId { get; set; }

        public string FunctionTypeName { get; set; }

        public Guid? PersonId { get; set; }

        public string PersonName { get; set; }

        public IList<Delegation> Delegations { get; set; }

        public Period Validity { get; set; }

        public BodyMandate(
            Guid bodyMandateId,
            Guid? organisationId,
            string organisationName,
            Guid? functionTypeId,
            string functionTypeName,
            Guid? personId,
            string personName,
            Period validity)
        {
            Delegations = new List<Delegation>();

            BodyMandateId = bodyMandateId;
            OrganisationId = organisationId;
            OrganisationName = organisationName;
            FunctionTypeId = functionTypeId;
            FunctionTypeName = functionTypeName;
            PersonId = personId;
            PersonName = personName;
            Validity = validity;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<BodyMandate> map)
            => map
                .Keyword(
                    k => k
                        .Name(p => p.BodyMandateId))
                .Keyword(
                    k => k
                        .Name(p => p.OrganisationId))
                .Text(
                    t => t
                        .Name(p => p.OrganisationName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Keyword(
                    k => k
                        .Name(p => p.FunctionTypeId))
                .Text(
                    t => t
                        .Name(p => p.FunctionTypeName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Keyword(
                    k => k
                        .Name(p => p.PersonId))
                .Text(
                    t => t
                        .Name(p => p.PersonName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Nested<Delegation>(
                    n => n
                        .Name(p => p.Delegations)
                        .IncludeInRoot()
                        .Properties(Delegation.Mapping))
                .Object<Period>(
                    o => o
                        .Name(p => p.Validity)
                        .Properties(Period.Mapping));
    }

    public class Delegation
    {
        public Guid DelegationAssignmentId { get; }

        public Guid PersonId { get; set; }

        public string PersonName { get; set; }

        public List<Contact> Contacts { get; set; }

        public Period Validity { get; set; }

        public Delegation(
            Guid delegationAssignmentId,
            Guid personId,
            string personName,
            List<Contact> contacts,
            Period validity)
        {
            Validity = validity;
            DelegationAssignmentId = delegationAssignmentId;
            PersonId = personId;
            PersonName = personName;
            Contacts = contacts;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<Delegation> map)
            => map
                .Keyword(
                    k => k
                        .Name(p => p.DelegationAssignmentId))
                .Keyword(
                    k => k
                        .Name(p => p.PersonId))
                .Text(
                    t => t
                        .Name(p => p.PersonName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Nested<Contact>(
                    n => n
                        .Name(p => p.Contacts)
                        .IncludeInRoot()
                        .Properties(Contact.Mapping))
                .Object<Period>(
                    o => o
                        .Name(p => p.Validity)
                        .Properties(Period.Mapping));
    }

    public class BodyLifecyclePhase
    {
        public Guid BodyLifecyclePhaseId { get; set; }

        public Guid LifecyclePhaseTypeId { get; set; }

        public string LifecyclePhaseTypeName { get; set; }

        public Period Validity { get; set; }

        public BodyLifecyclePhase(
            Guid bodyLifecyclePhaseId,
            Guid lifecyclePhaseTypeId,
            string lifecyclePhaseTypeName,
            Period validity)
        {
            BodyLifecyclePhaseId = bodyLifecyclePhaseId;
            LifecyclePhaseTypeId = lifecyclePhaseTypeId;
            LifecyclePhaseTypeName = lifecyclePhaseTypeName;
            Validity = validity;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<BodyLifecyclePhase> map)
            => map
                .Keyword(
                    k => k
                        .Name(p => p.BodyLifecyclePhaseId))
                .Keyword(
                    k => k
                        .Name(p => p.LifecyclePhaseTypeId))
                .Text(
                    t => t
                        .Name(p => p.LifecyclePhaseTypeName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Object<Period>(
                    o => o
                        .Name(p => p.Validity)
                        .Properties(Period.Mapping));
    }

    public class BodyOrganisation
    {
        public Guid BodyOrganisationId { get; set; }

        public Guid OrganisationId { get; set; }

        public string Name { get; set; }

        public Period Validity { get; set; }

        public BodyOrganisation(
            Guid bodyOrganisationId,
            Guid organisationId,
            string name,
            Period validity)
        {
            BodyOrganisationId = bodyOrganisationId;
            OrganisationId = organisationId;
            Name = name;
            Validity = validity;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<BodyOrganisation> map)
            => map
                .Keyword(
                    k => k
                        .Name(p => p.BodyOrganisationId))
                .Keyword(
                    k => k
                        .Name(p => p.OrganisationId))
                .Text(
                    t => t
                        .Name(p => p.Name)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Object<Period>(
                    o => o
                        .Name(p => p.Validity)
                        .Properties(Period.Mapping));
    }

    public class BodyFormalFramework
    {
        public BodyFormalFramework(
            Guid bodyFormalFrameworkId,
            Guid formalFrameworkId,
            string formalFrameworkName,
            Period validity)
        {
            BodyFormalFrameworkId = bodyFormalFrameworkId;

            FormalFrameworkId = formalFrameworkId;
            FormalFrameworkName = formalFrameworkName;

            Validity = validity;
        }

        public Guid BodyFormalFrameworkId { get; set; }

        public Guid FormalFrameworkId { get; set; }

        public string FormalFrameworkName { get; set; }

        public Period Validity { get; set; }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<BodyFormalFramework> map)
            => map
                .Keyword(k => k.Name(p => p.BodyFormalFrameworkId))
                .Keyword(k => k.Name(p => p.FormalFrameworkId))
                .Text(
                    t => t
                        .Name(p => p.FormalFrameworkName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Object<Period>(
                    o => o
                        .Name(p => p.Validity)
                        .Properties(Period.Mapping));
    }

    public class BodyParticipation
    {
        public Guid BodyParticipationId { get; set; }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<BodyParticipation> map)
            => map
                .Keyword(k => k.Name(p => p.BodyParticipationId));
    }

    public class BodyClassification
    {
        public BodyClassification(
            Guid bodyClassificationId,
            Guid classificationId,
            string classificationName,
            Guid classificationTypeId,
            string classificationTypeName,
            Period validity)
        {
            BodyClassificationId = bodyClassificationId;
            ClassificationId = classificationId;
            ClassificationName = classificationName;
            ClassificationTypeId = classificationTypeId;
            ClassificationTypeName = classificationTypeName;
            Validity = validity;
        }

        public Guid BodyClassificationId { get; set; }

        public Guid ClassificationId { get; set; }

        public string ClassificationName { get; set; }

        public Guid ClassificationTypeId { get; set; }

        public string ClassificationTypeName { get; set; }

        public Period Validity { get; set; }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<BodyClassification> map)
            => map
                .Keyword(k => k.Name(c => c.BodyClassificationId))
                .Keyword(k => k.Name(c => c.ClassificationId))
                .Keyword(k => k.Name(c => c.ClassificationTypeId))
                .Text(
                    t => t
                        .Name(p => p.ClassificationName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Text(
                    t => t
                        .Name(p => p.ClassificationTypeName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Object<Period>(
                    o => o
                        .Name(p => p.Validity)
                        .Properties(Period.Mapping));
    }
}
