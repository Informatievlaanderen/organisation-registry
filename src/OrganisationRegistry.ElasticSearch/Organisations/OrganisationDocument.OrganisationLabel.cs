namespace OrganisationRegistry.ElasticSearch.Organisations;

using Osc;
using System;
using Common;

public partial class OrganisationDocument
{
    public class OrganisationLabel
    {
        public Guid OrganisationLabelId { get; set; }
        public Guid LabelTypeId { get; set; }
        public string LabelTypeName { get; set; }
        public string Value { get; set; }
        public Period Validity { get; set; }

        protected OrganisationLabel()
        {
            LabelTypeName = string.Empty;
            Value = string.Empty;
            Validity = Period.Infinite();
        }

        public OrganisationLabel(
            Guid organisationLabelId,
            Guid labelTypeId,
            string labelTypeName,
            string value,
            Period validity)
        {
            OrganisationLabelId = organisationLabelId;
            LabelTypeId = labelTypeId;
            LabelTypeName = labelTypeName;
            Value = value;
            Validity = validity;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationLabel> map) => map
            .Keyword(k => k
                .Name(p => p.OrganisationLabelId))

            .Keyword(k => k
                .Name(p => p.LabelTypeId))

            .Text(t => t
                .Name(p => p.LabelTypeName)
                .Fields(x => x.Keyword(y => y.Name("keyword"))))

            .Text(t => t
                .Name(p => p.Value)
                .Fields(x => x.Keyword(y => y.Name("keyword"))))

            .Object<Period>(o => o
                .Name(p => p.Validity)
                .Properties(Period.Mapping));
    }
}
