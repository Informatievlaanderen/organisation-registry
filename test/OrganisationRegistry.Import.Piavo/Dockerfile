# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY .config/dotnet-tools.json ./.config/dotnet-tools.json
COPY paket.dependencies paket.lock ./
COPY *.pfx ./
COPY .paket/ ./.paket/
COPY SolutionInfo.cs ./
COPY src/ ./src/
COPY test/OrganisationRegistry.Import.Piavo/ ./test/OrganisationRegistry.Import.Piavo/

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet tool restore && \
    dotnet paket restore && \
    dotnet build src/OrganisationRegistry.Api/OrganisationRegistry.Api.csproj -c Release && \
    dotnet publish test/OrganisationRegistry.Import.Piavo/OrganisationRegistry.Import.Piavo.csproj \
    -c Release -o /out --no-dependencies -r linux-x64 --self-contained false -p:PublishReadyToRun=false

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /out .

ENTRYPOINT ["dotnet", "OrganisationRegistry.Import.Piavo.dll"]