namespace OrganisationRegistry.ElasticSearch.Projections.IndividualRebuild;

using OrganisationRegistry.ElasticSearch.Bodies;
using OrganisationRegistry.ElasticSearch.Organisations;
using OrganisationRegistry.ElasticSearch.People;
using OrganisationRegistry.ElasticSearch.Projections.Body;
using OrganisationRegistry.ElasticSearch.Projections.Organisations;
using OrganisationRegistry.ElasticSearch.Projections.People;
using OrganisationRegistry.SqlServer.ElasticSearchProjections;

public static class IndividualRebuildRunnerConfigs
{
    public static readonly IndividualRebuildRunnerConfig<OrganisationRegistry.Organisation.Organisation, OrganisationDocument, OrganisationToRebuild> Organisation =
        new(
            OrganisationsRunner.ElasticSearchProjectionsProjectionName,
            OrganisationsRunner.EventHandlers,
            ctx => ctx.OrganisationsToRebuild,
            row => row.OrganisationId);

    public static readonly IndividualRebuildRunnerConfig<OrganisationRegistry.Body.Body, BodyDocument, BodyToRebuild> Body =
        new(
            BodyRunner.ElasticSearchProjectionsProjectionName,
            BodyRunner.EventHandlers,
            ctx => ctx.BodiesToRebuild,
            row => row.BodyId);

    public static readonly IndividualRebuildRunnerConfig<OrganisationRegistry.Person.Person, PersonDocument, PersonToRebuild> Person =
        new(
            PeopleRunner.ElasticSearchProjectionsProjectionName,
            PeopleRunner.EventHandlers,
            ctx => ctx.PeopleToRebuild,
            row => row.PersonId);
}
