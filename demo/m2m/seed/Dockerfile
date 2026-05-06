FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY OrganisationRegistry.Demo.Seed.csproj ./
RUN dotnet restore
COPY Program.cs ./
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "OrganisationRegistry.Demo.Seed.dll"]
