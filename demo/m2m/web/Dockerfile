FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY m2m.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "m2m.dll"]
