namespace OrganisationRegistry.Api.Infrastructure.Swagger.Examples;

using System;
using System.Collections.Generic;
using SqlServer.BodyClassification;
using SqlServer.BodyClassificationType;
using SqlServer.Building;
using SqlServer.Capacity;
using SqlServer.ContactType;
using SqlServer.FormalFramework;
using SqlServer.FormalFrameworkCategory;
using SqlServer.FunctionType;
using SqlServer.KeyType;
using SqlServer.LabelType;
using SqlServer.LifecyclePhaseType;
using SqlServer.Location;
using SqlServer.LocationType;
using SqlServer.MandateRoleType;
using SqlServer.OrganisationClassification;
using SqlServer.OrganisationClassificationType;
using SqlServer.OrganisationRelationType;
using SqlServer.Purpose;
using SqlServer.RegulationSubTheme;
using SqlServer.RegulationTheme;
using SqlServer.SeatType;
using Swashbuckle.AspNetCore.Filters;

public class BodyClassificationListExamples : IExamplesProvider<List<BodyClassificationListItem>>
{
    public List<BodyClassificationListItem> GetExamples() =>
        new()
        {
            new BodyClassificationListItem
            {
                Id = new Guid("11111111-0000-0000-0000-000000000001"),
                Name = "Naam orgaanclassificatie",
                Order = 1,
                Active = true,
                BodyClassificationTypeId = new Guid("11111111-0000-0000-0000-000000000002"),
                BodyClassificationTypeName = "Naam orgaanclassificatietype",
            },
        };
}

public class BodyClassificationTypeListExamples : IExamplesProvider<List<BodyClassificationTypeListItem>>
{
    public List<BodyClassificationTypeListItem> GetExamples() =>
        new()
        {
            new BodyClassificationTypeListItem
            {
                Id = new Guid("22222222-0000-0000-0000-000000000001"),
                Name = "Naam orgaanclassificatietype",
            },
        };
}

public class BuildingListExamples : IExamplesProvider<List<BuildingListItem>>
{
    public List<BuildingListItem> GetExamples() =>
        new()
        {
            new BuildingListItem
            {
                Id = new Guid("33333333-0000-0000-0000-000000000001"),
                Name = "Naam gebouw",
                VimId = 12345,
            },
        };
}

public class CapacityListExamples : IExamplesProvider<List<CapacityListItem>>
{
    public List<CapacityListItem> GetExamples() =>
        new()
        {
            new CapacityListItem
            {
                Id = new Guid("44444444-0000-0000-0000-000000000001"),
                Name = "Naam hoedanigheid",
                IsRemoved = false,
            },
        };
}

public class ContactTypeListExamples : IExamplesProvider<List<ContactTypeListItem>>
{
    public List<ContactTypeListItem> GetExamples() =>
        new()
        {
            new ContactTypeListItem
            {
                Id = new Guid("55555555-0000-0000-0000-000000000001"),
                Name = "Naam contacttype",
                Regex = "Reguliere expressie voor validatie",
                Example = "Voorbeeld van een geldig contact",
            },
        };
}

public class FormalFrameworkListExamples : IExamplesProvider<List<FormalFrameworkListItem>>
{
    public List<FormalFrameworkListItem> GetExamples() =>
        new()
        {
            new FormalFrameworkListItem
            {
                Id = new Guid("66666666-0000-0000-0000-000000000001"),
                Name = "Naam toepassingsgebied",
                Code = "Code toepassingsgebied",
                FormalFrameworkCategoryId = new Guid("66666666-0000-0000-0000-000000000002"),
                FormalFrameworkCategoryName = "Naam toepassingsgebiedcategorie",
            },
        };
}

public class FormalFrameworkCategoryListExamples : IExamplesProvider<List<FormalFrameworkCategoryListItem>>
{
    public List<FormalFrameworkCategoryListItem> GetExamples() =>
        new()
        {
            new FormalFrameworkCategoryListItem
            {
                Id = new Guid("77777777-0000-0000-0000-000000000001"),
                Name = "Naam toepassingsgebiedcategorie",
            },
        };
}

public class FunctionTypeListExamples : IExamplesProvider<List<FunctionTypeListItem>>
{
    public List<FunctionTypeListItem> GetExamples() =>
        new()
        {
            new FunctionTypeListItem
            {
                Id = new Guid("88888888-0000-0000-0000-000000000001"),
                Name = "Naam functietype",
            },
        };
}

public class KeyTypeListExamples : IExamplesProvider<List<KeyTypeListItem>>
{
    public List<KeyTypeListItem> GetExamples() =>
        new()
        {
            new KeyTypeListItem
            {
                Id = new Guid("99999999-0000-0000-0000-000000000001"),
                Name = "Naam sleuteltype",
                IsRemoved = false,
            },
        };
}

public class LabelTypeListExamples : IExamplesProvider<List<LabelTypeListItem>>
{
    public List<LabelTypeListItem> GetExamples() =>
        new()
        {
            new LabelTypeListItem
            {
                Id = new Guid("aaaaaaaa-0000-0000-0000-000000000001"),
                Name = "Naam benamingstype",
            },
        };
}

public class LifecyclePhaseTypeListExamples : IExamplesProvider<List<LifecyclePhaseTypeListItem>>
{
    public List<LifecyclePhaseTypeListItem> GetExamples() =>
        new()
        {
            new LifecyclePhaseTypeListItem
            {
                Id = new Guid("bbbbbbbb-0000-0000-0000-000000000001"),
                Name = "Naam levensloopfasetype",
                RepresentsActivePhase = true,
                IsDefaultPhase = true,
            },
        };
}

public class LocationListExamples : IExamplesProvider<List<LocationListItem>>
{
    public List<LocationListItem> GetExamples() =>
        new()
        {
            new LocationListItem
            {
                Id = new Guid("cccccccc-0000-0000-0000-000000000001"),
                CrabLocationId = "CRAB-locatie-ID",
                FormattedAddress = "Straat Huisnummer, Postcode Gemeente",
                Street = "Straatnaam en huisnummer",
                ZipCode = "Postcode",
                City = "Gemeente",
                Country = "Land",
                HasCrabLocation = true,
            },
        };
}

public class LocationTypeListExamples : IExamplesProvider<List<LocationTypeListItem>>
{
    public List<LocationTypeListItem> GetExamples() =>
        new()
        {
            new LocationTypeListItem
            {
                Id = new Guid("dddddddd-0000-0000-0000-000000000001"),
                Name = "Naam locatietype",
            },
        };
}

public class MandateRoleTypeListExamples : IExamplesProvider<List<MandateRoleTypeListItem>>
{
    public List<MandateRoleTypeListItem> GetExamples() =>
        new()
        {
            new MandateRoleTypeListItem
            {
                Id = new Guid("eeeeeeee-0000-0000-0000-000000000001"),
                Name = "Naam mandaat roltype",
            },
        };
}

public class OrganisationClassificationListExamples : IExamplesProvider<List<OrganisationClassificationListItem>>
{
    public List<OrganisationClassificationListItem> GetExamples() =>
        new()
        {
            new OrganisationClassificationListItem
            {
                Id = new Guid("ffffffff-0000-0000-0000-000000000001"),
                Name = "Naam organisatieclassificatie",
                Order = 1,
                ExternalKey = null,
                Active = true,
                OrganisationClassificationTypeId = new Guid("ffffffff-0000-0000-0000-000000000002"),
                OrganisationClassificationTypeName = "Naam organisatieclassificatietype",
            },
        };
}

public class OrganisationClassificationTypeListExamples : IExamplesProvider<List<OrganisationClassificationTypeListItem>>
{
    public List<OrganisationClassificationTypeListItem> GetExamples() =>
        new()
        {
            new OrganisationClassificationTypeListItem
            {
                Id = new Guid("11111111-1111-0000-0000-000000000001"),
                Name = "Naam organisatieclassificatietype",
                AllowDifferentClassificationsToOverlap = false,
            },
        };
}

public class OrganisationRelationTypeListExamples : IExamplesProvider<List<OrganisationRelationTypeListItem>>
{
    public List<OrganisationRelationTypeListItem> GetExamples() =>
        new()
        {
            new OrganisationRelationTypeListItem
            {
                Id = new Guid("22222222-2222-0000-0000-000000000001"),
                Name = "Naam organisatierelatietype",
                InverseName = "Inverse naam organisatierelatietype",
            },
        };
}

public class PurposeListExamples : IExamplesProvider<List<PurposeListItem>>
{
    public List<PurposeListItem> GetExamples() =>
        new()
        {
            new PurposeListItem
            {
                Id = new Guid("33333333-3333-0000-0000-000000000001"),
                Name = "Naam beleidsveld",
            },
        };
}

public class RegulationSubThemeListExamples : IExamplesProvider<List<RegulationSubThemeListItem>>
{
    public List<RegulationSubThemeListItem> GetExamples() =>
        new()
        {
            new RegulationSubThemeListItem
            {
                Id = new Guid("44444444-4444-0000-0000-000000000001"),
                Name = "Naam regelgevingsubthema",
                RegulationThemeId = new Guid("44444444-4444-0000-0000-000000000002"),
                RegulationThemeName = "Naam regelgevingsthema",
            },
        };
}

public class RegulationThemeListExamples : IExamplesProvider<List<RegulationThemeListItem>>
{
    public List<RegulationThemeListItem> GetExamples() =>
        new()
        {
            new RegulationThemeListItem
            {
                Id = new Guid("55555555-5555-0000-0000-000000000001"),
                Name = "Naam regelgevingsthema",
            },
        };
}

public class SeatTypeListExamples : IExamplesProvider<List<SeatTypeListItem>>
{
    public List<SeatTypeListItem> GetExamples() =>
        new()
        {
            new SeatTypeListItem
            {
                Id = new Guid("66666666-6666-0000-0000-000000000001"),
                Name = "Naam zeteltype",
                Order = 1,
                IsEffective = true,
            },
        };
}
