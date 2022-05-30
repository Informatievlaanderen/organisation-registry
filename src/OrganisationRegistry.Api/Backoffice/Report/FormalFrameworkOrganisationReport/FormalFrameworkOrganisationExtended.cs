namespace OrganisationRegistry.Api.Backoffice.Report.FormalFrameworkOrganisationReport;

using System;
using System.ComponentModel;
using System.Linq;
using Infrastructure;
using ElasticSearch.Organisations;
using OrganisationRegistry.Infrastructure.Configuration;

public class FormalFrameworkOrganisationExtended : FormalFrameworkOrganisationBase
{
    [Order]
    [DisplayName("INR")]
    public string? INR { get; set; }

    [Order]
    [DisplayName("KBO")]
    public string KBO { get; set; }

    [Order]
    [DisplayName("Orafin")]
    public string? Orafin { get; set; }

    [Order]
    [DisplayName("Vlimpers")]
    public string? Vlimpers { get; set; }

    [Order]
    [DisplayName("Vlimpers_kort")]
    public string? VlimpersKort { get; set; }

    [Order]
    [DisplayName("Bestuursniveau")]
    public string? Bestuursniveau { get; set; }

    [Order]
    [DisplayName("Categorie")]
    public string? Categorie { get; set; }

    [Order]
    [DisplayName("Entiteitsvorm")]
    public string? Entiteitsvorm { get; set; }

    [Order]
    [DisplayName("ESR Klasse toezichthoudende overheid")]
    public string? ESRKlasseToezichthoudendeOverheid { get; set; }

    [Order]
    [DisplayName("ESR Sector")]
    public string? ESRSector { get; set; }

    [Order]
    [DisplayName("ESR Toezichthoudende overheid")]
    public string? ESRToezichthoudendeOverheid { get; set; }

    [Order]
    [DisplayName("Uitvoerend niveau")]
    public string? UitvoerendNiveau { get; set; }

    [Order]
    [DisplayName("Geldig vanaf")]
    public DateTime? ValidFrom { get; set; }

    [Order]
    [DisplayName("Geldig tot")]
    public DateTime? ValidTo { get; set; }

    [Order]
    [DisplayName("Organisatietype mandaten- en vermogensaangifte")]
    public string? OrganisatieTypeMandatenEnVermogensaangifte { get; set; }

    public FormalFrameworkOrganisationExtended(
        OrganisationDocument document,
        ApiConfigurationSection @params,
        DateTime today)
        : base(document, @params, today)
    {
        INR = document.Keys
            .SingleOrDefault(x =>
                x.KeyTypeId == @params.INR_KeyTypeId &&
                x.Validity.OverlapsWith(today))
            ?.Value;

        KBO = document.KboNumber;

        Orafin = document.Keys
            .SingleOrDefault(x =>
                x.KeyTypeId == @params.Orafin_KeyTypeId &&
                x.Validity.OverlapsWith(today))
            ?.Value;

        Vlimpers = document.Keys
            .SingleOrDefault(x =>
                x.KeyTypeId == @params.Vlimpers_KeyTypeId &&
                x.Validity.OverlapsWith(today))
            ?.Value;

        VlimpersKort = document.Keys
            .SingleOrDefault(x =>
                x.KeyTypeId == @params.VlimpersKort_KeyTypeId &&
                x.Validity.OverlapsWith(today))
            ?.Value;

        Bestuursniveau = document.OrganisationClassifications
            .SingleOrDefault(x =>
                x.OrganisationClassificationTypeId == @params.Bestuursniveau_ClassificationTypeId &&
                x.Validity.OverlapsWith(today))
            ?.OrganisationClassificationName;

        Categorie = document.OrganisationClassifications
            .SingleOrDefault(x =>
                x.OrganisationClassificationTypeId == @params.Categorie_ClassificationTypeId &&
                x.Validity.OverlapsWith(today))
            ?.OrganisationClassificationName;

        Entiteitsvorm = document.OrganisationClassifications
            .SingleOrDefault(x =>
                x.OrganisationClassificationTypeId == @params.Entiteitsvorm_ClassificationTypeId &&
                x.Validity.OverlapsWith(today))
            ?.OrganisationClassificationName;

        ESRKlasseToezichthoudendeOverheid = document.OrganisationClassifications
            .SingleOrDefault(x =>
                x.OrganisationClassificationTypeId ==
                @params.ESRKlasseToezichthoudendeOverheid_ClassificationTypeId &&
                x.Validity.OverlapsWith(today))
            ?.OrganisationClassificationName;

        ESRSector = document.OrganisationClassifications
            .SingleOrDefault(x =>
                x.OrganisationClassificationTypeId == @params.ESRSector_ClassificationTypeId &&
                x.Validity.OverlapsWith(today))
            ?.OrganisationClassificationName;

        ESRToezichthoudendeOverheid = document.OrganisationClassifications
            .SingleOrDefault(x =>
                x.OrganisationClassificationTypeId == @params.ESRToezichthoudendeOverheid_ClassificationTypeId &&
                x.Validity.OverlapsWith(today))
            ?.OrganisationClassificationName;

        UitvoerendNiveau = document.OrganisationClassifications
            .SingleOrDefault(x =>
                x.OrganisationClassificationTypeId == @params.UitvoerendNiveau_ClassificationTypeId &&
                x.Validity.OverlapsWith(today))
            ?.OrganisationClassificationName;

        OrganisatieTypeMandatenEnVermogensaangifte = document.OrganisationClassifications
            .SingleOrDefault(x =>
                x.OrganisationClassificationTypeId == @params.Organisatietype_Mandaten_En_Vermogensaangifte_ClassificationTypeId &&
                x.Validity.OverlapsWith(today))
            ?.OrganisationClassificationName;

        ValidFrom = document.Validity.Start;

        ValidTo = document.Validity.End;
    }
}