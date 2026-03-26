# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Kopieer tool manifest en installeer paket
COPY .config/dotnet-tools.json ./.config/dotnet-tools.json
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet tool restore

# Kopieer paket bestanden
COPY paket.dependencies paket.lock ./
COPY .paket/ ./.paket/

# Kopieer source
COPY SolutionInfo.cs ./
COPY organisationregistry-api.pfx ./
COPY src/ ./src/

# Restore packages via paket (gebruikt NuGet cache mount)
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet paket restore

# Build en publish
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish src/OrganisationRegistry.Api/OrganisationRegistry.Api.csproj \
    -c Release -o /out -p:PublishReadyToRun=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app/src/OrganisationRegistry.Api
COPY --from=build /out .
COPY --from=build /src/organisationregistry-api.pfx /app/organisationregistry-api.pfx

EXPOSE 9002
ENV ASPNETCORE_URLS=http://+:9002

ENTRYPOINT ["dotnet", "OrganisationRegistry.Api.dll"]
