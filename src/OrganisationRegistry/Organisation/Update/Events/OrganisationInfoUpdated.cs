namespace OrganisationRegistry.Organisation.Events;

using System;
using System.Collections.Generic;

public class OrganisationInfoUpdated : BaseEvent<OrganisationInfoUpdated>
{
    public Guid OrganisationId => Id;

    public string Name { get; }
    public string? Article { get; }
    public string PreviousName { get; }

    public string Description { get; }
    public string PreviousDescription { get; }

    public string OvoNumber { get; }

    public string ShortName { get; }
    public string PreviousShortName { get; }

    public List<Purpose> Purposes { get; }
    public List<Purpose> PreviousPurposes { get; }

    public bool ShowOnVlaamseOverheidSites { get; }
    public bool PreviouslyShownInVlaanderenBe { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public DateTime? OperationalValidFrom { get; }
    public DateTime? PreviouslyOperationalValidFrom { get; }

    public DateTime? OperationalValidTo { get; }
    public DateTime? PreviouslyOperationalValidTo { get; }


    [Obsolete("Use events for individual property changes, eg: OrganisationValidityChanged")]
    public OrganisationInfoUpdated(
        Guid organisationId,
        string name,
        string? article,
        string description,
        string ovoNumber,
        string shortName,
        List<Purpose> purposes,
        bool showOnVlaamseOverheidSites,
        DateTime? validFrom,
        DateTime? validTo,
        DateTime? operationalValidFrom,
        DateTime? operationalValidTo,
        string previousName,
        string previousDescription,
        string previousShortName,
        List<Purpose> previousPurposes,
        bool previouslyShownInVlaanderenBe,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo,
        DateTime? previouslyOperationalValidFrom,
        DateTime? previouslyOperationalValidTo)
    {
        Id = organisationId;

        Name = name;
        Article = article;
        Description = description;
        OvoNumber = ovoNumber;
        ShortName = shortName;
        Purposes = purposes;
        ShowOnVlaamseOverheidSites = showOnVlaamseOverheidSites;
        ValidFrom = validFrom;
        ValidTo = validTo;
        OperationalValidFrom = operationalValidFrom;
        OperationalValidTo = operationalValidTo;

        PreviousName = previousName;
        PreviousDescription = previousDescription;
        PreviousShortName = previousShortName;
        PreviousPurposes = previousPurposes;
        PreviouslyShownInVlaanderenBe = previouslyShownInVlaanderenBe;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
        PreviouslyOperationalValidFrom = previouslyOperationalValidFrom;
        PreviouslyOperationalValidTo = previouslyOperationalValidTo;
    }
}