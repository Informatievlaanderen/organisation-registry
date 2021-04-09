namespace OrganisationRegistry.Infrastructure.Configuration
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Newtonsoft.Json;

    public class ApiConfiguration
    {
        public static string Section = "Api";

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Created => DateTime.Now;

        public string Title { get; set; }
        public string Description { get; set; }

        public string CorsOrigin { get; set; }

        public string EnvironmentName { get; set; }
        public string GraphiteAddress { get; set; }

        public Guid EmailContactTypeId { get; set; }
        public Guid PhoneContactTypeId { get; set; }
        public Guid CellPhoneContactTypeId { get; set; }

        //classification type ids
        public Guid LegalFormClassificationTypeId { get; set; }
        public Guid PolicyDomainClassificationTypeId { get; set; }
        public Guid ResponsibleMinisterClassificationTypeId { get; set; }
        public Guid BodyPolicyDomainClassificationTypeId { get; set; }
        public Guid BodyResponsibleMinisterClassificationTypeId { get; set; }

        public Guid Bestuursniveau_ClassificationTypeId { get; set; }
        public Guid Categorie_ClassificationTypeId { get; set; }
        public Guid Entiteitsvorm_ClassificationTypeId { get; set; }
        public Guid ESRKlasseToezichthoudendeOverheid_ClassificationTypeId { get; set; }
        public Guid ESRSector_ClassificationTypeId { get; set; }
        public Guid ESRToezichthoudendeOverheid_ClassificationTypeId { get; set; }
        public Guid UitvoerendNiveau_ClassificationTypeId { get; set; }

        //key type ids
        public Guid VademecumKeyTypeId { get; set; }

        public Guid INR_KeyTypeId { get; set; }

        [Obsolete("Replaced by property on Organisation entity.")]
        public Guid KBO_KeyTypeId { get; set; }
        public Guid Orafin_KeyTypeId { get; set; }
        public Guid Vlimpers_KeyTypeId { get; set; }
        public Guid VlimpersKort_KeyTypeId { get; set; }

        public string DataVlaanderenOrganisationUri { get; set; }

        public Guid FrenchLabelTypeId { get; set; }
        public Guid GermanLabelTypeId { get; set; }
        public Guid EnglishLabelTypeId { get; set; }

        public string VademecumReport_FormalFrameworkIds { get; set; }

        public Guid Mep_FormalFrameworkId { get; set; }

        public Guid RegisteredOfficeLocationTypeId { get; set; }

        public string KboSender { get; set; }
        public string KboRecipient { get; set; }
        public string KboMagdaEndpoint { get; set; }
        public string KboCertificate { get; set; }
        public int KboMagdaTimeout { get; set; }
        public string RijksRegisterCertificatePwd { get; set; }

        public int SyncFromKboBatchSize { get; set; }

        public Guid KboV2FormalNameLabelTypeId { get; set; }
        public Guid KboV2RegisteredOfficeLocationTypeId { get; set; }
        public Guid KboV2LegalFormOrganisationClassificationTypeId { get; set; }
        public string RepertoriumMagdaEndpoint { get; set; }
        public string RepertoriumCapacity { get; set; }
    }
}
