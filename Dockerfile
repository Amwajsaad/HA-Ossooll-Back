FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY . .

RUN dotnet restore HA-Ossooll.API/HA-Ossooll.API.csproj
RUN dotnet publish HA-Ossooll.API/HA-Ossooll.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "HA-Ossooll.API.dll"]
