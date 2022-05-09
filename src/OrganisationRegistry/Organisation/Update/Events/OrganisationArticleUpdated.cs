namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationArticleUpdated : BaseEvent<OrganisationArticleUpdated>
{
    public Guid OrganisationId
        => Id;

    public string? Article { get; }

    public OrganisationArticleUpdated(
        Guid organisationId,
        string? article)
    {
        Id = organisationId;

        Article = article;
    }
}
