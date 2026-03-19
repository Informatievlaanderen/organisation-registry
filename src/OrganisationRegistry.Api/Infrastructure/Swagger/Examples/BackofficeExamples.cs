namespace OrganisationRegistry.Api.Infrastructure.Swagger.Examples;

using System;
using System.Collections.Generic;
using OrganisationRegistry.Body;
using OrganisationRegistry.Person;
using SqlServer.Body;
using SqlServer.BodyClassification;
using SqlServer.BodyClassificationType;
using SqlServer.Building;
using SqlServer.Capacity;
using SqlServer.ContactType;
using SqlServer.DelegationAssignments;
using SqlServer.Delegations;
using SqlServer.Event;
using SqlServer.Configuration;
using SqlServer.FormalFramework;
using SqlServer.FormalFrameworkCategory;
using SqlServer.FunctionType;
using SqlServer.KeyType;
using SqlServer.LabelType;
using SqlServer.LifecyclePhaseType;
using SqlServer.Location;
using SqlServer.LocationType;
using SqlServer.MandateRoleType;
using SqlServer.Organisation;
using SqlServer.OrganisationClassification;
using SqlServer.OrganisationClassificationType;
using SqlServer.OrganisationRelationType;
using SqlServer.Person;
using SqlServer.ProjectionState;
using SqlServer.Purpose;
using SqlServer.RegulationSubTheme;
using SqlServer.RegulationTheme;
using SqlServer.SeatType;
using Swashbuckle.AspNetCore.Filters;

// ──────────────────────────────────────────────────────────────────────────────
// Organisaties
// ──────────────────────────────────────────────────────────────────────────────

public class OrganisationListExamples : IExamplesProvider<List<OrganisationListItem>>
{
    public List<OrganisationListItem> GetExamples() =>
        new()
        {
            new OrganisationListItem
            {
                Id = 1,
                OrganisationId = new Guid("aaaaaaaa-0001-0000-0000-000000000001"),
                OvoNumber = "OVO000001",
                Name = "Naam organisatie",
                ShortName = "Afkorting organisatie",
                ParentOrganisation = "Naam bovenliggende organisatie",
                ParentOrganisationId = new Guid("aaaaaaaa-0001-0000-0000-000000000002"),
                ParentOrganisationOvoNumber = "OVO000002",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class OrganisationBankAccountListExamples : IExamplesProvider<List<OrganisationBankAccountListItem>>
{
    public List<OrganisationBankAccountListItem> GetExamples() =>
        new()
        {
            new OrganisationBankAccountListItem
            {
                OrganisationBankAccountId = new Guid("aaaaaaaa-0002-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-0002-0000-0000-000000000002"),
                BankAccountNumber = "BE00 0000 0000 0000",
                IsIban = true,
                Bic = "GEBABEBB",
                IsBic = true,
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
                Source = null,
            },
        };
}

public class OrganisationBodyListExamples : IExamplesProvider<List<OrganisationBodyListItem>>
{
    public List<OrganisationBodyListItem> GetExamples() =>
        new()
        {
            new OrganisationBodyListItem
            {
                OrganisationBodyId = new Guid("aaaaaaaa-0003-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-0003-0000-0000-000000000002"),
                BodyId = new Guid("aaaaaaaa-0003-0000-0000-000000000003"),
                BodyName = "Naam orgaan",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class OrganisationBuildingListExamples : IExamplesProvider<List<OrganisationBuildingListItem>>
{
    public List<OrganisationBuildingListItem> GetExamples() =>
        new()
        {
            new OrganisationBuildingListItem
            {
                OrganisationBuildingId = new Guid("aaaaaaaa-0004-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-0004-0000-0000-000000000002"),
                BuildingId = new Guid("aaaaaaaa-0004-0000-0000-000000000003"),
                BuildingName = "Naam gebouw",
                IsMainBuilding = true,
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class OrganisationCapacityListExamples : IExamplesProvider<List<OrganisationCapacityListItem>>
{
    public List<OrganisationCapacityListItem> GetExamples() =>
        new()
        {
            new OrganisationCapacityListItem
            {
                OrganisationCapacityId = new Guid("aaaaaaaa-0005-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-0005-0000-0000-000000000002"),
                CapacityId = new Guid("aaaaaaaa-0005-0000-0000-000000000003"),
                CapacityName = "Naam hoedanigheid",
                PersonId = new Guid("aaaaaaaa-0005-0000-0000-000000000004"),
                PersonName = "Naam persoon",
                FunctionId = new Guid("aaaaaaaa-0005-0000-0000-000000000005"),
                FunctionName = "Naam functie",
                LocationId = new Guid("aaaaaaaa-0005-0000-0000-000000000006"),
                LocationName = "Naam locatie",
                ContactsJson = null,
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
                IsActive = true,
                ScheduledForRemoval = false,
            },
        };
}

public class OrganisationChildListExamples : IExamplesProvider<List<OrganisationChildListItem>>
{
    public List<OrganisationChildListItem> GetExamples() =>
        new()
        {
            new OrganisationChildListItem
            {
                OrganisationOrganisationParentId = new Guid("aaaaaaaa-0006-0000-0000-000000000001"),
                Id = new Guid("aaaaaaaa-0006-0000-0000-000000000002"),
                Name = "Naam onderliggende organisatie",
                OvoNumber = "OVO000003",
                ParentOrganisationId = new Guid("aaaaaaaa-0006-0000-0000-000000000003"),
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
                OrganisationValidFrom = new DateTime(2018, 1, 1),
                OrganisationValidTo = null,
            },
        };
}

public class OrganisationContactListExamples : IExamplesProvider<List<OrganisationContactListItem>>
{
    public List<OrganisationContactListItem> GetExamples() =>
        new()
        {
            new OrganisationContactListItem
            {
                OrganisationContactId = new Guid("aaaaaaaa-0007-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-0007-0000-0000-000000000002"),
                ContactTypeId = new Guid("aaaaaaaa-0007-0000-0000-000000000003"),
                ContactTypeName = "Naam contacttype",
                ContactValue = "Contactwaarde",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class OrganisationFormalFrameworkListExamples : IExamplesProvider<List<OrganisationFormalFrameworkListItem>>
{
    public List<OrganisationFormalFrameworkListItem> GetExamples() =>
        new()
        {
            new OrganisationFormalFrameworkListItem
            {
                OrganisationFormalFrameworkId = new Guid("aaaaaaaa-0008-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-0008-0000-0000-000000000002"),
                FormalFrameworkId = new Guid("aaaaaaaa-0008-0000-0000-000000000003"),
                FormalFrameworkName = "Naam toepassingsgebied",
                ParentOrganisationId = new Guid("aaaaaaaa-0008-0000-0000-000000000004"),
                ParentOrganisationName = "Naam bovenliggende organisatie",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class OrganisationFunctionListExamples : IExamplesProvider<List<OrganisationFunctionListItem>>
{
    public List<OrganisationFunctionListItem> GetExamples() =>
        new()
        {
            new OrganisationFunctionListItem
            {
                OrganisationFunctionId = new Guid("aaaaaaaa-0009-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-0009-0000-0000-000000000002"),
                FunctionId = new Guid("aaaaaaaa-0009-0000-0000-000000000003"),
                FunctionName = "Naam functie",
                PersonId = new Guid("aaaaaaaa-0009-0000-0000-000000000004"),
                PersonName = "Naam persoon",
                ContactsJson = null,
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class OrganisationKeyListExamples : IExamplesProvider<List<OrganisationKeyListItem>>
{
    public List<OrganisationKeyListItem> GetExamples() =>
        new()
        {
            new OrganisationKeyListItem
            {
                OrganisationKeyId = new Guid("aaaaaaaa-000a-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-000a-0000-0000-000000000002"),
                KeyTypeId = new Guid("aaaaaaaa-000a-0000-0000-000000000003"),
                KeyTypeName = "Naam sleuteltype",
                KeyValue = "Sleutelwaarde",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
                ScheduledForRemoval = false,
            },
        };
}

public class OrganisationTerminationListExamples : IExamplesProvider<List<OrganisationTerminationListItem>>
{
    public List<OrganisationTerminationListItem> GetExamples() =>
        new()
        {
            new OrganisationTerminationListItem
            {
                Id = new Guid("aaaaaaaa-000b-0000-0000-000000000001"),
                OvoNumber = "OVO000004",
                KboNumber = "KBO-nummer organisatie",
                Name = "Naam organisatie",
                Status = TerminationStatus.Proposed,
                Date = new DateTime(2023, 12, 31),
                Code = "Code stopzetting",
                Reason = "Reden stopzetting",
            },
        };
}

public class OrganisationLabelListExamples : IExamplesProvider<List<OrganisationLabelListItem>>
{
    public List<OrganisationLabelListItem> GetExamples() =>
        new()
        {
            new OrganisationLabelListItem
            {
                OrganisationLabelId = new Guid("aaaaaaaa-000c-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-000c-0000-0000-000000000002"),
                LabelTypeId = new Guid("aaaaaaaa-000c-0000-0000-000000000003"),
                LabelTypeName = "Naam benamingstype",
                LabelValue = "Waarde benaming",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
                Source = null,
            },
        };
}

public class OrganisationLocationListExamples : IExamplesProvider<List<OrganisationLocationListItem>>
{
    public List<OrganisationLocationListItem> GetExamples() =>
        new()
        {
            new OrganisationLocationListItem
            {
                OrganisationLocationId = new Guid("aaaaaaaa-000d-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-000d-0000-0000-000000000002"),
                LocationId = new Guid("aaaaaaaa-000d-0000-0000-000000000003"),
                LocationName = "Straat Huisnummer, Postcode Gemeente",
                LocationTypeId = new Guid("aaaaaaaa-000d-0000-0000-000000000004"),
                LocationTypeName = "Naam locatietype",
                IsMainLocation = true,
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
                Source = null,
            },
        };
}

public class OrganisationOpeningHourListExamples : IExamplesProvider<List<OrganisationOpeningHourListItem>>
{
    public List<OrganisationOpeningHourListItem> GetExamples() =>
        new()
        {
            new OrganisationOpeningHourListItem
            {
                OrganisationOpeningHourId = new Guid("aaaaaaaa-000e-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-000e-0000-0000-000000000002"),
                Opens = new TimeSpan(9, 0, 0),
                Closes = new TimeSpan(17, 0, 0),
                DayOfWeek = DayOfWeek.Monday,
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class OrganisationOrganisationClassificationListExamples : IExamplesProvider<List<OrganisationOrganisationClassificationListItem>>
{
    public List<OrganisationOrganisationClassificationListItem> GetExamples() =>
        new()
        {
            new OrganisationOrganisationClassificationListItem
            {
                OrganisationOrganisationClassificationId = new Guid("aaaaaaaa-000f-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-000f-0000-0000-000000000002"),
                OrganisationClassificationTypeId = new Guid("aaaaaaaa-000f-0000-0000-000000000003"),
                OrganisationClassificationTypeName = "Naam organisatieclassificatietype",
                OrganisationClassificationId = new Guid("aaaaaaaa-000f-0000-0000-000000000004"),
                OrganisationClassificationName = "Naam organisatieclassificatie",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
                Source = null,
            },
        };
}

public class OrganisationParentListExamples : IExamplesProvider<List<OrganisationParentListItem>>
{
    public List<OrganisationParentListItem> GetExamples() =>
        new()
        {
            new OrganisationParentListItem
            {
                OrganisationOrganisationParentId = new Guid("aaaaaaaa-0010-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-0010-0000-0000-000000000002"),
                ParentOrganisationId = new Guid("aaaaaaaa-0010-0000-0000-000000000003"),
                ParentOrganisationName = "Naam bovenliggende organisatie",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class OrganisationRegulationListExamples : IExamplesProvider<List<OrganisationRegulationListItem>>
{
    public List<OrganisationRegulationListItem> GetExamples() =>
        new()
        {
            new OrganisationRegulationListItem
            {
                OrganisationRegulationId = new Guid("aaaaaaaa-0011-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-0011-0000-0000-000000000002"),
                RegulationThemeId = new Guid("aaaaaaaa-0011-0000-0000-000000000003"),
                RegulationThemeName = "Naam regelgevingsthema",
                RegulationSubThemeId = new Guid("aaaaaaaa-0011-0000-0000-000000000004"),
                RegulationSubThemeName = "Naam regelgevingsubthema",
                Name = "Naam regelgeving",
                Date = new DateTime(2020, 1, 1),
                Url = "URL regelgeving",
                WorkRulesUrl = "URL arbeidsreglement",
                Description = "Beschrijving regelgeving",
                DescriptionRendered = "<p>Beschrijving regelgeving</p>",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class OrganisationRelationListExamples : IExamplesProvider<List<OrganisationRelationListItem>>
{
    public List<OrganisationRelationListItem> GetExamples() =>
        new()
        {
            new OrganisationRelationListItem
            {
                OrganisationRelationId = new Guid("aaaaaaaa-0012-0000-0000-000000000001"),
                OrganisationId = new Guid("aaaaaaaa-0012-0000-0000-000000000002"),
                RelationId = new Guid("aaaaaaaa-0012-0000-0000-000000000003"),
                RelationName = "Naam organisatierelatietype",
                RelationInverseName = "Inverse naam organisatierelatietype",
                RelatedOrganisationId = new Guid("aaaaaaaa-0012-0000-0000-000000000004"),
                RelatedOrganisationName = "Naam gerelateerde organisatie",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

// ──────────────────────────────────────────────────────────────────────────────
// Organen
// ──────────────────────────────────────────────────────────────────────────────

public class BodyListExamples : IExamplesProvider<List<BodyListItem>>
{
    public List<BodyListItem> GetExamples() =>
        new()
        {
            new BodyListItem
            {
                Id = new Guid("bbbbbbbb-0001-0000-0000-000000000001"),
                BodyNumber = "OR-0001",
                Name = "Naam orgaan",
                ShortName = "Afkorting orgaan",
                Organisation = "Naam organisatie",
                OrganisationId = new Guid("bbbbbbbb-0001-0000-0000-000000000002"),
            },
        };
}

public class BodyBodyClassificationListExamples : IExamplesProvider<List<BodyBodyClassificationListItem>>
{
    public List<BodyBodyClassificationListItem> GetExamples() =>
        new()
        {
            new BodyBodyClassificationListItem
            {
                BodyBodyClassificationId = new Guid("bbbbbbbb-0002-0000-0000-000000000001"),
                BodyId = new Guid("bbbbbbbb-0002-0000-0000-000000000002"),
                BodyClassificationTypeId = new Guid("bbbbbbbb-0002-0000-0000-000000000003"),
                BodyClassificationTypeName = "Naam orgaanclassificatietype",
                BodyClassificationId = new Guid("bbbbbbbb-0002-0000-0000-000000000004"),
                BodyClassificationName = "Naam orgaanclassificatie",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class BodyContactListExamples : IExamplesProvider<List<BodyContactListItem>>
{
    public List<BodyContactListItem> GetExamples() =>
        new()
        {
            new BodyContactListItem
            {
                BodyContactId = new Guid("bbbbbbbb-0003-0000-0000-000000000001"),
                BodyId = new Guid("bbbbbbbb-0003-0000-0000-000000000002"),
                ContactTypeId = new Guid("bbbbbbbb-0003-0000-0000-000000000003"),
                ContactTypeName = "Naam contacttype",
                ContactValue = "Contactwaarde",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class BodyFormalFrameworkListExamples : IExamplesProvider<List<BodyFormalFrameworkListItem>>
{
    public List<BodyFormalFrameworkListItem> GetExamples() =>
        new()
        {
            new BodyFormalFrameworkListItem
            {
                BodyFormalFrameworkId = new Guid("bbbbbbbb-0004-0000-0000-000000000001"),
                BodyId = new Guid("bbbbbbbb-0004-0000-0000-000000000002"),
                FormalFrameworkId = new Guid("bbbbbbbb-0004-0000-0000-000000000003"),
                FormalFrameworkName = "Naam toepassingsgebied",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class BodyLifecyclePhaseListExamples : IExamplesProvider<List<BodyLifecyclePhaseListItem>>
{
    public List<BodyLifecyclePhaseListItem> GetExamples() =>
        new()
        {
            new BodyLifecyclePhaseListItem
            {
                BodyLifecyclePhaseId = new Guid("bbbbbbbb-0005-0000-0000-000000000001"),
                BodyId = new Guid("bbbbbbbb-0005-0000-0000-000000000002"),
                LifecyclePhaseTypeId = new Guid("bbbbbbbb-0005-0000-0000-000000000003"),
                LifecyclePhaseTypeName = "Naam levensloopfasetype",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
                HasAdjacentGaps = false,
            },
        };
}

public class BodyMandateListExamples : IExamplesProvider<List<BodyMandateListItem>>
{
    public List<BodyMandateListItem> GetExamples() =>
        new()
        {
            new BodyMandateListItem
            {
                BodyMandateId = new Guid("bbbbbbbb-0006-0000-0000-000000000001"),
                BodyMandateType = BodyMandateType.Person,
                BodyId = new Guid("bbbbbbbb-0006-0000-0000-000000000002"),
                BodySeatId = new Guid("bbbbbbbb-0006-0000-0000-000000000003"),
                BodySeatNumber = "Zetelcode orgaan",
                BodySeatName = "Naam zetel",
                BodySeatTypeId = new Guid("bbbbbbbb-0006-0000-0000-000000000004"),
                BodySeatTypeName = "Naam zeteltype",
                BodySeatTypeOrder = 1,
                DelegatorId = new Guid("bbbbbbbb-0006-0000-0000-000000000005"),
                DelegatorName = "Naam delegerende organisatie",
                DelegatedId = new Guid("bbbbbbbb-0006-0000-0000-000000000006"),
                DelegatedName = "Naam gedelegeerde",
                AssignedToId = new Guid("bbbbbbbb-0006-0000-0000-000000000007"),
                AssignedToName = "Naam toegewezene",
                ContactsJson = null,
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class BodyOrganisationListExamples : IExamplesProvider<List<BodyOrganisationListItem>>
{
    public List<BodyOrganisationListItem> GetExamples() =>
        new()
        {
            new BodyOrganisationListItem
            {
                BodyOrganisationId = new Guid("bbbbbbbb-0007-0000-0000-000000000001"),
                BodyId = new Guid("bbbbbbbb-0007-0000-0000-000000000002"),
                OrganisationId = new Guid("bbbbbbbb-0007-0000-0000-000000000003"),
                OrganisationName = "Naam organisatie",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class BodySeatListExamples : IExamplesProvider<List<BodySeatListItem>>
{
    public List<BodySeatListItem> GetExamples() =>
        new()
        {
            new BodySeatListItem
            {
                BodySeatId = new Guid("bbbbbbbb-0008-0000-0000-000000000001"),
                BodyId = new Guid("bbbbbbbb-0008-0000-0000-000000000002"),
                Name = "Naam zetel",
                BodySeatNumber = "Zetelcode orgaan",
                PaidSeat = false,
                EntitledToVote = true,
                SeatTypeId = new Guid("bbbbbbbb-0008-0000-0000-000000000003"),
                SeatTypeName = "Naam zeteltype",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

// ──────────────────────────────────────────────────────────────────────────────
// Personen
// ──────────────────────────────────────────────────────────────────────────────

public class PersonListExamples : IExamplesProvider<List<PersonListItem>>
{
    public List<PersonListItem> GetExamples() =>
        new()
        {
            new PersonListItem
            {
                Id = new Guid("cccccccc-0001-0000-0000-000000000001"),
                FirstName = "Voornaam persoon",
                Name = "Achternaam persoon",
                FullName = "Voornaam persoon Achternaam persoon",
                Sex = Sex.Female,
                DateOfBirth = new DateTime(1985, 6, 15),
            },
        };
}

public class PersonCapacityListExamples : IExamplesProvider<List<PersonCapacityListItem>>
{
    public List<PersonCapacityListItem> GetExamples() =>
        new()
        {
            new PersonCapacityListItem
            {
                OrganisationCapacityId = new Guid("cccccccc-0002-0000-0000-000000000001"),
                OrganisationId = new Guid("cccccccc-0002-0000-0000-000000000002"),
                OrganisationName = "Naam organisatie",
                CapacityId = new Guid("cccccccc-0002-0000-0000-000000000003"),
                CapacityName = "Naam hoedanigheid",
                PersonId = new Guid("cccccccc-0002-0000-0000-000000000004"),
                FunctionId = new Guid("cccccccc-0002-0000-0000-000000000005"),
                FunctionName = "Naam functie",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class PersonFunctionListExamples : IExamplesProvider<List<PersonFunctionListItem>>
{
    public List<PersonFunctionListItem> GetExamples() =>
        new()
        {
            new PersonFunctionListItem
            {
                OrganisationFunctionId = new Guid("cccccccc-0003-0000-0000-000000000001"),
                OrganisationId = new Guid("cccccccc-0003-0000-0000-000000000002"),
                OrganisationName = "Naam organisatie",
                FunctionId = new Guid("cccccccc-0003-0000-0000-000000000003"),
                FunctionName = "Naam functie",
                PersonId = new Guid("cccccccc-0003-0000-0000-000000000004"),
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class PersonMandateListExamples : IExamplesProvider<List<PersonMandateListItem>>
{
    public List<PersonMandateListItem> GetExamples() =>
        new()
        {
            new PersonMandateListItem
            {
                PersonMandateId = new Guid("cccccccc-0004-0000-0000-000000000001"),
                BodyMandateId = new Guid("cccccccc-0004-0000-0000-000000000002"),
                DelegationAssignmentId = null,
                BodyId = new Guid("cccccccc-0004-0000-0000-000000000003"),
                BodyName = "Naam orgaan",
                BodyOrganisationId = new Guid("cccccccc-0004-0000-0000-000000000004"),
                BodyOrganisationName = "Naam organisatie orgaan",
                BodySeatId = new Guid("cccccccc-0004-0000-0000-000000000005"),
                BodySeatName = "Naam zetel",
                BodySeatNumber = "Zetelcode orgaan",
                PaidSeat = false,
                OrganisationId = new Guid("cccccccc-0004-0000-0000-000000000006"),
                OrganisationName = "Naam organisatie",
                FunctionTypeId = new Guid("cccccccc-0004-0000-0000-000000000007"),
                FunctionTypeName = "Naam functietype",
                PersonId = new Guid("cccccccc-0004-0000-0000-000000000008"),
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

// ──────────────────────────────────────────────────────────────────────────────
// Delegaties
// ──────────────────────────────────────────────────────────────────────────────

public class DelegationListExamples : IExamplesProvider<List<DelegationListItem>>
{
    public List<DelegationListItem> GetExamples() =>
        new()
        {
            new DelegationListItem
            {
                Id = new Guid("dddddddd-0001-0000-0000-000000000001"),
                OrganisationId = new Guid("dddddddd-0001-0000-0000-000000000002"),
                OrganisationName = "Naam organisatie",
                FunctionTypeId = new Guid("dddddddd-0001-0000-0000-000000000003"),
                FunctionTypeName = "Naam functietype",
                BodyId = new Guid("dddddddd-0001-0000-0000-000000000004"),
                BodyName = "Naam orgaan",
                BodyOrganisationId = new Guid("dddddddd-0001-0000-0000-000000000005"),
                BodyOrganisationName = "Naam organisatie orgaan",
                BodySeatId = new Guid("dddddddd-0001-0000-0000-000000000006"),
                BodySeatName = "Naam zetel",
                BodySeatNumber = "Zetelcode orgaan",
                BodySeatTypeId = new Guid("dddddddd-0001-0000-0000-000000000007"),
                BodySeatTypeName = "Naam zeteltype",
                IsDelegated = true,
                NumberOfDelegationAssignments = 1,
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

public class DelegationAssignmentListExamples : IExamplesProvider<List<DelegationAssignmentListItem>>
{
    public List<DelegationAssignmentListItem> GetExamples() =>
        new()
        {
            new DelegationAssignmentListItem
            {
                Id = new Guid("dddddddd-0002-0000-0000-000000000001"),
                BodyId = new Guid("dddddddd-0002-0000-0000-000000000002"),
                BodySeatId = new Guid("dddddddd-0002-0000-0000-000000000003"),
                BodyMandateId = new Guid("dddddddd-0002-0000-0000-000000000004"),
                PersonId = new Guid("dddddddd-0002-0000-0000-000000000005"),
                PersonName = "Naam persoon",
                ContactsJson = null,
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
            },
        };
}

// ──────────────────────────────────────────────────────────────────────────────
// Administratie
// ──────────────────────────────────────────────────────────────────────────────

public class EventListExamples : IExamplesProvider<List<EventListItem>>
{
    public List<EventListItem> GetExamples() =>
        new()
        {
            new EventListItem
            {
                Id = new Guid("eeeeeeee-0001-0000-0000-000000000001"),
                Number = 1,
                Version = 0,
                Name = "Naam event",
                Timestamp = new DateTime(2024, 1, 1, 10, 0, 0),
                Data = "{}",
                Ip = "IP-adres gebruiker",
                LastName = "Achternaam gebruiker",
                FirstName = "Voornaam gebruiker",
                UserId = "Gebruikers-ID",
            },
        };
}

public class ConfigurationListExamples : IExamplesProvider<List<ConfigurationListItem>>
{
    public List<ConfigurationListItem> GetExamples() =>
        new()
        {
            new ConfigurationListItem
            {
                Key = "Configuratiesleutel",
                Description = "Beschrijving configuratie",
                Value = "Waarde configuratie",
            },
        };
}
