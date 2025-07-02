FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Modules/Maraudr.Document/Maraudr.Document.Endpoints/Maraudr.Document.Endpoints.csproj", "Modules/Maraudr.Document/Maraudr.Document.Endpoints/"]
RUN dotnet restore "Modules/Maraudr.Document/Maraudr.Document.Endpoints/Maraudr.Document.Endpoints.csproj"
COPY . .
WORKDIR "/src/Modules/Maraudr.Document/Maraudr.Document.Endpoints"
RUN dotnet build "./Maraudr.Document.Endpoints.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Maraudr.Document.Endpoints.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Maraudr.Document.Endpoints.dll"]
