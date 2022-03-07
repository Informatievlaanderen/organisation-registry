﻿namespace OrganisationRegistry.ElasticSearch.Organisations
{
    using System;
    using Common;
    using Osc;

    public partial class OrganisationDocument
    {
        public class OrganisationRegulation
        {
            public Guid OrganisationRegulationId { get; set; }
            public Guid? RegulationThemeId { get; set; }
            public string? RegulationThemeName { get; set; }
            public Guid? RegulationSubThemeId { get; set; }
            public string? RegulationSubThemeName { get; set; }
            public string Name { get; set; }
            public DateTime? Date { get; set; }
            public string? Url { get; set; }
            public string? Description { get; set; }
            public string? DescriptionRendered { get; set; }
            public Period Validity { get; set; }

            protected OrganisationRegulation()
            {
            }

            public OrganisationRegulation(
                Guid organisationRegulationId,
                Guid? regulationThemeId,
                string? regulationThemeName,
                Guid? regulationSubThemeId,
                string? regulationSubThemeName,
                string name,
                string? description,
                string? descriptionRendered,
                string? url,
                DateTime? date,
                Period validity)
            {
                OrganisationRegulationId = organisationRegulationId;
                RegulationThemeId = regulationThemeId;
                RegulationThemeName = regulationThemeName;
                RegulationSubThemeId = regulationSubThemeId;
                RegulationSubThemeName = regulationSubThemeName;
                Name = name;
                Date = date;
                Url = url;
                Description = description;
                DescriptionRendered = descriptionRendered;
                Validity = validity;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationRegulation> map)
                => map
                    .Keyword(
                        k => k
                            .Name(p => p.OrganisationRegulationId))
                    .Keyword(
                        k => k
                            .Name(p => p.RegulationThemeId))
                    .Text(
                        t => t
                            .Name(p => p.RegulationThemeName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Keyword(
                        k => k
                            .Name(p => p.RegulationSubThemeId))
                    .Text(
                        t => t
                            .Name(p => p.RegulationSubThemeName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Text(
                        t => t
                            .Name(p => p.Name)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Date(
                        d => d
                            .Name(p => p.Date)
                            .Format("yyyy-MM-dd'T'HH:mm:ss||yyyy-MM-dd HH:mm:ss||yyyy-MM-dd||epoch_millis"))
                    .Text(
                        t => t
                            .Name(p => p.Url)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Text(
                        t => t
                            .Name(p => p.Description)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Text(
                        t => t
                            .Name(p => p.DescriptionRendered)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Object<Period>(
                        o => o
                            .Name(p => p.Validity)
                            .Properties(Period.Mapping));
        }
    }
}