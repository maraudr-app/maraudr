FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore Maraudr.User.Endpoints/Maraudr.User.Endpoints.csproj

WORKDIR /src/Maraudr.User.Endpoints
RUN dotnet publish Maraudr.User.Endpoints.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Maraudr.User.Endpoints.dll"]
