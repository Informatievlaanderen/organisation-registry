# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Kopieer tool manifest en source
COPY .config/dotnet-tools.json ./.config/dotnet-tools.json
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY SolutionInfo.cs ./
COPY src/ ./src/

# Tool restore + publish in één stap zodat de NuGet cache
# nooit een incomplete tool-installatie kan cachen.
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet tool restore && \
    dotnet publish src/OrganisationRegistry.ElasticSearch.Projections/OrganisationRegistry.ElasticSearch.Projections.csproj \
    -c Release -o /out -p:PublishReadyToRun=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app/src/OrganisationRegistry.ElasticSearch.Projections
COPY --from=build /out .

ENTRYPOINT ["dotnet", "OrganisationRegistry.ElasticSearch.Projections.dll"]
