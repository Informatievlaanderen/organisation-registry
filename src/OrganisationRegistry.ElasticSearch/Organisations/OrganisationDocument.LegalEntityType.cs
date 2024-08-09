namespace OrganisationRegistry.ElasticSearch.Organisations;

using Osc;

public class LegalEntityType
{
    public LegalEntityType(string code, string description)
    {
        Code = code;
        Description = description;
    }

    public LegalEntityType()
    {
        Code = string.Empty;
        Description = string.Empty;
    }

    public string Code { get; set; }
    public string Description { get; set; }

    public static IPromise<IProperties> Mapping(PropertiesDescriptor<LegalEntityType> map)
        => map
            .Keyword(
                k => k
                    .Name(p => p.Code)).Keyword(
                k => k
                    .Name(p => p.Description));
}
