namespace OrganisationRegistry.Organisation.State;

using System;
using System.Collections.Generic;

public class OrganisationState
{
    public string Name { get; set; }
    public string OvoNumber { get; set; }
    public string? ShortName { get; set; }
    public Article Article { get; set; }
    public string? Description { get; set; }
    public Period Validity { get; set; }
    public Period OperationalValidity { get; set; }
    public bool ShowOnVlaamseOverheidSites { get; set; }
    public bool IsActive { get; set; }
    public List<OrganisationKey> OrganisationKeys { get; set; }
    public OrganisationContacts OrganisationContacts { get; }
    public List<OrganisationLabel> OrganisationLabels { get; }
    public List<OrganisationOrganisationClassification> OrganisationOrganisationClassifications { get; }
    public List<OrganisationFunction> OrganisationFunctionTypes { get; }
    public List<OrganisationRelation> OrganisationRelations { get; }
    public List<OrganisationCapacity> OrganisationCapacities { get; set; }
    public List<OrganisationParent> OrganisationParents { get; }
    public OrganisationList<OrganisationFormalFramework> OrganisationFormalFrameworks { get; }
    public OrganisationBankAccounts OrganisationBankAccounts { get; }
    public List<OrganisationOpeningHour> OrganisationOpeningHours { get; }
    public Dictionary<Guid, OrganisationFormalFramework> OrganisationFormalFrameworkParentsPerFormalFramework { get; }
    public OrganisationBuildings OrganisationBuildings { get; }
    public OrganisationLocations OrganisationLocations { get; }
    public List<OrganisationRegulation> OrganisationRegulations { get; }
    public bool UnderVlimpersManagement { get; set; }


    public OrganisationState()
    {
        Name = string.Empty;
        OvoNumber = string.Empty;
        ShortName = string.Empty;
        Description = string.Empty;
        Validity = new Period();
        OperationalValidity = new Period();
        Article = Article.None;

        OrganisationKeys = new List<OrganisationKey>();
        OrganisationContacts = new OrganisationContacts();
        OrganisationLabels = new List<OrganisationLabel>();
        OrganisationOrganisationClassifications = new List<OrganisationOrganisationClassification>();
        OrganisationFunctionTypes = new List<OrganisationFunction>();
        OrganisationRelations = new List<OrganisationRelation>();
        OrganisationCapacities = new List<OrganisationCapacity>();
        OrganisationParents = new List<OrganisationParent>();
        OrganisationFormalFrameworks = new OrganisationList<OrganisationFormalFramework>();
        OrganisationBankAccounts = new OrganisationBankAccounts();
        OrganisationOpeningHours = new List<OrganisationOpeningHour>();
        OrganisationFormalFrameworkParentsPerFormalFramework = new Dictionary<Guid, OrganisationFormalFramework>();
        OrganisationBuildings = new OrganisationBuildings();
        OrganisationLocations = new OrganisationLocations();
        OrganisationRegulations = new List<OrganisationRegulation>();

        UnderVlimpersManagement = false;
    }
}
