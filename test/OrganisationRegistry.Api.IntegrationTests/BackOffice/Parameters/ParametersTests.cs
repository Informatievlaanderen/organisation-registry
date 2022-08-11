namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Parameters;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class ParametersTests
{
    private readonly ApiFixture _apiFixture;

    public ParametersTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ParametersTestData.KeytypesRoute)]
    [InlineData(ParametersTestData.LifecyclephasetypesRoute)]
    [InlineData(ParametersTestData.LabeltypesRoute)]
    [InlineData(ParametersTestData.BodyclassificationtypesRoute)]
    [InlineData(ParametersTestData.CapacitiesRoute)]
    [InlineData(ParametersTestData.FormalframeworkcategoriesRoute)]
    [InlineData(ParametersTestData.FunctiontypesRoute)]
    [InlineData(ParametersTestData.LocationtypesRoute)]
    [InlineData(ParametersTestData.MandateroletypesRoute)]
    [InlineData(ParametersTestData.PurposesRoute)]
    [InlineData(ParametersTestData.RegulationthemesRoute)]
    [InlineData(ParametersTestData.LocationsRoute)]
    [InlineData(ParametersTestData.OrganisationrelationtypesRoute)]
    [InlineData(ParametersTestData.SeattypesRoute)]
    [InlineData(ParametersTestData.BuildingsRoute)]
    [InlineData(ParametersTestData.OrganisationclassificationtypesRoute)]
    [InlineData(ParametersTestData.FormalframeworksRoute)]
    [InlineData(ParametersTestData.RegulationsubthemesRoute)]
    [InlineData(ParametersTestData.BodyclassificationsRoute)]
    [InlineData(ParametersTestData.OrganisationclassificationsRoute)]
    public async Task TestParameter(string route)
    {
        var testData = ParametersTestData.ParametersToTest[route];

        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{route}/{_apiFixture.Fixture.Create<Guid>()}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // CREATE
        await CreateWithInvalidDataAndVerifyBadRequest(route);

        var (id, createResponseDictionary) = await CreateAndVerify(route, testData.CreateParameterRequestType, testData.DependencyRoutes);

        // UPDATE
        var updateResponseDictionary = await UpdateAndVerify(route, id, testData.CreateParameterRequestType, testData.DependencyRoutes);

        await UpdateWithInvalidDataAndVerifyBadRequest(route, id);

        // LIST
        await GetListAndVerify(route);

        if (!testData.SupportsRemoval)
            return;

        createResponseDictionary["isRemoved"].Should().Be(false);
        updateResponseDictionary["isRemoved"].Should().Be(false);

        var removeResponseDictionary = await RemoveAndVerify(route, id);
        removeResponseDictionary["isRemoved"].Should().Be(true);
    }

    private async Task CreateWithInvalidDataAndVerifyBadRequest(string baseRoute)
    {
        // create
        var createResponse = await ApiFixture.Post(_apiFixture.HttpClient, $"{baseRoute}", "prut");
        await ApiFixture.VerifyStatusCode(createResponse, HttpStatusCode.BadRequest);

    }

    private async Task<(Guid, Dictionary<string, object>)> CreateAndVerify(
        string baseRoute,
        Type requestType,
        ImmutableList<string> dependencyRoutes)
    {
        var (request, createResponse) = await CreateWithDependencies(baseRoute, requestType, dependencyRoutes);
        await ApiFixture.VerifyStatusCode(createResponse, HttpStatusCode.Created);

        // get
        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, createResponse.Headers.Location!.ToString());
        await ApiFixture.VerifyStatusCode(getResponse, HttpStatusCode.OK);

        return (request.Id, await ApiFixture.ValidateResponse(requestType, getResponse, request));
    }

    private async Task<Dictionary<string, object>> UpdateAndVerify(
        string baseRoute,
        Guid id,
        Type createRequestType,
        ImmutableList<string> dependencyRoutes)
    {
        var updatedInstance = await CreateInstanceWithDependencies(createRequestType, dependencyRoutes);
        updatedInstance.Id = id;

        // update
        var updateResponse = await Update(baseRoute, id, updatedInstance);
        await ApiFixture.VerifyStatusCode(updateResponse, HttpStatusCode.OK);

        // get
        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{baseRoute}/{id}");
        await ApiFixture.VerifyStatusCode(getResponse, HttpStatusCode.OK);

        return await ApiFixture.ValidateResponse(createRequestType, getResponse, updatedInstance);
    }

    private async Task UpdateWithInvalidDataAndVerifyBadRequest(string baseRoute, Guid id)
    {
        // update
        var updateResponse = await ApiFixture.Put(_apiFixture.HttpClient, $"{baseRoute}/{id}", "prut");
        await ApiFixture.VerifyStatusCode(updateResponse, HttpStatusCode.BadRequest);
    }

    private async Task GetListAndVerify(string route)
    {
        var testData = ParametersTestData.ParametersToTest[route];

        await CreateWithDependencies(route, testData.CreateParameterRequestType, testData.DependencyRoutes);
        await CreateWithDependencies(route, testData.CreateParameterRequestType, testData.DependencyRoutes);

        await VerifyGetList(route);
        await VerifyGetList(route.Replace("/v1/", "/v1/parameters/"));
    }

    private async Task VerifyGetList(string route)
    {
        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{route}");
        await ApiFixture.VerifyStatusCode(getResponse, HttpStatusCode.OK);

        var deserializedResponse = await ApiFixture.DeserializeAsList(getResponse);
        deserializedResponse.Should().NotBeNull();

        deserializedResponse.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    private async Task<Dictionary<string, object>> RemoveAndVerify(string baseRoute, Guid id)
    {
        // remove
        var deleteResponse = await ApiFixture.Delete(_apiFixture.HttpClient, $"{baseRoute}/{id}");
        await ApiFixture.VerifyStatusCode(deleteResponse, HttpStatusCode.NoContent);

        // get
        var getResponse = await ApiFixture.Get(_apiFixture.HttpClient, $"{baseRoute}/{id}");
        await ApiFixture.VerifyStatusCode(getResponse, HttpStatusCode.OK);

        return await ApiFixture.Deserialize(getResponse);
    }

    private async Task<(dynamic, HttpResponseMessage)> CreateWithDependencies(
        string requestRoute,
        Type requestType,
        ImmutableList<string> dependencyRoutes)
    {
        var request = await CreateInstanceWithDependencies(requestType, dependencyRoutes);

        var response = await Create(requestRoute, request);

        return (request, response);
    }

    private async Task<dynamic> CreateInstanceWithDependencies(
        Type requestType,
        ImmutableList<string> dependencyRoutes)
    {
        var request = _apiFixture.CreateInstanceOf(requestType);

        foreach (var route in dependencyRoutes)
        {
            var dependency = ParametersTestData.ParametersToTestDependencies[route];

            request = await AddDependency(requestType, route, dependency.PropertyName, dependency.Type, request);
        }

        return request;
    }

    private async Task<object> AddDependency(Type requestType, string dependencyRoute, string dependencyProperty, Type dependencyRequestType, object request)
    {
        var dependencyRequest = _apiFixture.CreateInstanceOf(dependencyRequestType);
        var dependencyResponse = await Create(dependencyRoute, dependencyRequest);

        requestType.InvokeMember(
            dependencyProperty,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
            Type.DefaultBinder,
            request,
            new object[] { ApiFixture.GetIdFrom(dependencyResponse.Headers) });

        return request;
    }

    public async Task<HttpResponseMessage> Create(string baseRoute, object body)
        => await ApiFixture.Post(_apiFixture.HttpClient, $"{baseRoute}", body);

    public async Task<HttpResponseMessage> Update(string baseRoute, Guid id, object updateRequest)
        => await ApiFixture.Put(_apiFixture.HttpClient, $"{baseRoute}/{id}", updateRequest);
}
