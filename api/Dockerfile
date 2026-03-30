# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Kopieer tool manifest, paket bestanden en source
COPY .config/dotnet-tools.json ./.config/dotnet-tools.json
COPY paket.dependencies paket.lock ./
COPY .paket/ ./.paket/
COPY SolutionInfo.cs ./
COPY organisationregistry-api.pfx ./
COPY src/ ./src/

# Tool restore + paket restore + publish in één stap zodat de NuGet cache
# nooit een incomplete tool-installatie kan cachen.
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet tool restore && \
    dotnet paket restore && \
    dotnet publish src/OrganisationRegistry.Api/OrganisationRegistry.Api.csproj \
    -c Release -o /out -p:PublishReadyToRun=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app/src/OrganisationRegistry.Api
COPY --from=build /out .
COPY --from=build /src/organisationregistry-api.pfx /app/organisationregistry-api.pfx

EXPOSE 9002
ENV ASPNETCORE_URLS=http://+:9002

ENTRYPOINT ["dotnet", "OrganisationRegistry.Api.dll"]
