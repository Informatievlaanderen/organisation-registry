namespace OrganisationRegistry.ElasticSearch;

using System;

public interface IDocument
{
    Guid Id { get; set; }

    string Name { get; set; }
    int ChangeId { get; set; }

    DateTimeOffset ChangeTime { get; set; }
}