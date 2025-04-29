FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore Maraudr.Stock.Endpoints/Maraudr.Stock.Endpoints.csproj

WORKDIR /src/Maraudr.Stock.Endpoints
RUN dotnet publish Maraudr.Stock.Endpoints.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Maraudr.Stock.Endpoints.dll"]
