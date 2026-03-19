namespace OrganisationRegistry.Api.Infrastructure.Swagger.Examples;

using System;
using System.Collections.Generic;
using Backoffice.Report.Participation;
using Backoffice.Report.BuildingOrganisationReport;
using Backoffice.Report.CapacityPersonReport;
using Backoffice.Report.FormalFrameworkBodyReport;
using Backoffice.Report.FormalFrameworkOrganisationReport;
using Backoffice.Report.FormalFrameworkParticipationReport;
using Backoffice.Report.OrganisationClassificationReport;
using Swashbuckle.AspNetCore.Filters;

public class BodyParticipationListExamples : IExamplesProvider<List<BodyParticipation>>
{
    public List<BodyParticipation> GetExamples() =>
        new()
        {
            new BodyParticipation
            {
                BodyId = Guid.NewGuid(),
                BodyName = "Naam orgaan",
                IsEffective = true,
                IsEffectiveTranslation = "Effectief",
                MaleCount = 3,
                FemaleCount = 2,
                UnknownCount = 0,
                TotalCount = 5,
                AssignedCount = 5,
                UnassignedCount = 0,
                MalePercentage = 60,
                FemalePercentage = 40,
                UnknownPercentage = 0,
                TotalCompliance = BodyParticipationCompliance.Compliant,
                FemaleCompliance = BodyParticipationCompliance.Compliant,
                MaleCompliance = BodyParticipationCompliance.Compliant,
            },
        };
}

public class BuildingOrganisationListExamples : IExamplesProvider<List<BuildingOrganisation>>
{
    public List<BuildingOrganisation> GetExamples() =>
        new()
        {
            new BuildingOrganisation
            {
                ParentOrganisationId = Guid.NewGuid(),
                ParentOrganisationName = "Naam moeder-organisatie",
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "Naam organisatie",
                OrganisationShortName = "Korte naam",
                OrganisationOvoNumber = "OVO-nummer",
                DataVlaanderenOrganisationUri = new Uri("https://data.vlaanderen.be/id/organisatie/OVO-nummer"),
                LegalForm = "Juridische vorm",
                PolicyDomain = "Beleidsdomein",
                ResponsibleMinister = "Bevoegde minister",
            },
        };
}

public class CapacityPersonListExamples : IExamplesProvider<List<CapacityPerson>>
{
    public List<CapacityPerson> GetExamples() =>
        new()
        {
            new CapacityPerson
            {
                ParentOrganisationId = Guid.NewGuid(),
                ParentOrganisationName = "Naam moeder-organisatie",
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "Naam organisatie",
                OvoNumber = "OVO-nummer",
                OrganisationShortName = "Korte naam",
                PersonId = Guid.NewGuid(),
                PersonName = "Naam persoon",
                FunctionTypeId = Guid.NewGuid(),
                FunctionTypeName = "Naam functietype",
                Email = "e-mailadres",
                Location = "Locatie",
                Phone = "Telefoonnummer",
                CellPhone = "Gsm-nummer",
                PolicyDomain = "Beleidsdomein",
            },
        };
}

public class FormalFrameworkBodyListExamples : IExamplesProvider<List<FormalFrameworkBody>>
{
    public List<FormalFrameworkBody> GetExamples() =>
        new()
        {
            new FormalFrameworkBody
            {
                BodyId = Guid.NewGuid(),
                BodyName = "Naam orgaan",
                BodyShortName = "Korte naam orgaan",
                BodyNumber = "Nummer orgaan",
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "Naam organisatie",
            },
        };
}

public class FormalFrameworkOrganisationBaseListExamples : IExamplesProvider<List<FormalFrameworkOrganisationBase>>
{
    public List<FormalFrameworkOrganisationBase> GetExamples() =>
        new()
        {
            new FormalFrameworkOrganisationBase
            {
                ParentOrganisationId = Guid.NewGuid(),
                ParentOrganisationName = "Naam moeder-organisatie",
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "Naam organisatie",
                OrganisationShortName = "Korte naam",
                OrganisationOvoNumber = "OVO-nummer",
                DataVlaanderenOrganisationUri = new Uri("https://data.vlaanderen.be/id/organisatie/OVO-nummer"),
                LegalForm = "Juridische vorm",
                PolicyDomain = "Beleidsdomein",
                ResponsibleMinister = "Bevoegde minister",
                MainLocation = "Hoofdlocatie",
                Location = "Locatie",
            },
        };
}

public class FormalFrameworkOrganisationExtendedListExamples : IExamplesProvider<List<FormalFrameworkOrganisationExtended>>
{
    public List<FormalFrameworkOrganisationExtended> GetExamples() =>
        new()
        {
            new FormalFrameworkOrganisationExtended
            {
                ParentOrganisationId = Guid.NewGuid(),
                ParentOrganisationName = "Naam moeder-organisatie",
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "Naam organisatie",
                OrganisationShortName = "Korte naam",
                OrganisationOvoNumber = "OVO-nummer",
                DataVlaanderenOrganisationUri = new Uri("https://data.vlaanderen.be/id/organisatie/OVO-nummer"),
                LegalForm = "Juridische vorm",
                PolicyDomain = "Beleidsdomein",
                ResponsibleMinister = "Bevoegde minister",
                MainLocation = "Hoofdlocatie",
                Location = "Locatie",
                INR = "INR-nummer",
                KBO = "KBO-nummer",
                Orafin = "Orafin-code",
                Vlimpers = "Vlimpers-code",
                VlimpersKort = "Vlimpers-kort-code",
                Bestuursniveau = "Bestuursniveau",
                Categorie = "Categorie",
                Entiteitsvorm = "Entiteitsvorm",
                ESRKlasseToezichthoudendeOverheid = "ESR klasse toezichthoudende overheid",
                ESRSector = "ESR sector",
                ESRToezichthoudendeOverheid = "ESR toezichthoudende overheid",
                UitvoerendNiveau = "Uitvoerend niveau",
                ValidFrom = new DateTime(2020, 1, 1),
                ValidTo = null,
                OrganisatieTypeMandatenEnVermogensaangifte = "Organisatietype mandaten en vermogensaangifte",
            },
        };
}

public class FormalFrameworkOrganisationVademecumListExamples : IExamplesProvider<List<FormalFrameworkOrganisationVademecum>>
{
    public List<FormalFrameworkOrganisationVademecum> GetExamples() =>
        new()
        {
            new FormalFrameworkOrganisationVademecum
            {
                ParentOrganisationId = Guid.NewGuid(),
                ParentOrganisationName = "Naam moeder-organisatie",
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "Naam organisatie",
                OrganisationShortName = "Korte naam",
                OrganisationOvoNumber = "OVO-nummer",
                DataVlaanderenOrganisationUri = new Uri("https://data.vlaanderen.be/id/organisatie/OVO-nummer"),
                VademecumKey = "Vademecumsleutel",
            },
        };
}

public class FormalFrameworkParticipationListExamples : IExamplesProvider<List<FormalFrameworkParticipation>>
{
    public List<FormalFrameworkParticipation> GetExamples() =>
        new()
        {
            new FormalFrameworkParticipation
            {
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "Naam organisatie",
                BodyId = Guid.NewGuid(),
                BodyName = "Naam orgaan",
                IsEffective = true,
                IsEffectiveTranslation = "Effectief",
                MaleCount = 3,
                FemaleCount = 2,
                UnknownCount = 0,
                TotalCount = 5,
                AssignedCount = 5,
                UnassignedCount = 0,
                MalePercentage = 60,
                FemalePercentage = 40,
                UnknownPercentage = 0,
            },
        };
}

public class ParticipationSummaryListExamples : IExamplesProvider<List<ParticipationSummary>>
{
    public List<ParticipationSummary> GetExamples() =>
        new()
        {
            new ParticipationSummary
            {
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "Naam organisatie",
                BodyId = Guid.NewGuid(),
                BodyName = "Naam orgaan",
                EffectiveMaleCount = 3,
                EffectiveFemaleCount = 2,
                EffectiveUnknownCount = 0,
                EffectiveTotalCount = 5,
                SubsidiaryMaleCount = 1,
                SubsidiaryFemaleCount = 1,
                SubsidiaryUnknownCount = 0,
                SubsidiaryTotalCount = 2,
                TotalCount = 7,
                AssignedCount = 7,
                UnassignedCount = 0,
                EffectiveMalePercentage = 60,
                EffectiveFemalePercentage = 40,
                EffectiveUnknownPercentage = 0,
                SubsidiaryMalePercentage = 50,
                SubsidiaryFemalePercentage = 50,
                SubsidiaryUnknownPercentage = 0,
                IsTotalCompliant = true,
                IsEffectiveCompliant = true,
                IsSubsidiaryCompliant = true,
                PolicyDomainClassificationId = Guid.NewGuid(),
                PolicyDomainClassificationName = "Naam beleidsdomeinclassificatie",
                ResponsibleMinisterClassificationId = Guid.NewGuid(),
                ResponsibleMinisterClassificationName = "Naam bevoegde-ministerclassificatie",
            },
        };
}

public class ClassificationOrganisationListExamples : IExamplesProvider<List<ClassificationOrganisation>>
{
    public List<ClassificationOrganisation> GetExamples() =>
        new()
        {
            new ClassificationOrganisation
            {
                ParentOrganisationId = Guid.NewGuid(),
                ParentOrganisationName = "Naam moeder-organisatie",
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "Naam organisatie",
                OrganisationShortName = "Korte naam",
                OrganisationNameFrench = "Naam organisatie (FR)",
                OrganisationNameEnglish = "Naam organisatie (EN)",
                OrganisationNameGerman = "Naam organisatie (DE)",
            },
        };
}

public class ClassificationOrganisationParticipationListExamples : IExamplesProvider<List<ClassificationOrganisationParticipation>>
{
    public List<ClassificationOrganisationParticipation> GetExamples() =>
        new()
        {
            new ClassificationOrganisationParticipation
            {
                OrganisationId = Guid.NewGuid(),
                OrganisationName = "Naam organisatie",
                BodyId = Guid.NewGuid(),
                BodyName = "Naam orgaan",
                MaleCount = 3,
                FemaleCount = 2,
                UnknownCount = 0,
                TotalCount = 5,
                AssignedCount = 5,
                UnassignedCount = 0,
                MalePercentage = 60,
                FemalePercentage = 40,
                UnknownPercentage = 0,
            },
        };
}
