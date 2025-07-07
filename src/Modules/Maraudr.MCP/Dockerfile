FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /source
COPY . .

# Restaurer les dépendances de tous les projets avant la compilation
RUN dotnet restore "Maraudr.MCP.Endpoints/Maraudr.MCP.Endpoints.csproj" && \
    dotnet restore "Maraudr.MCP.Server/Maraudr.MCP.Server.csproj"

# Compiler l'application principale
WORKDIR /source/Maraudr.MCP.Endpoints
ARG TARGETARCH
RUN dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# Compiler le serveur MCP
WORKDIR /source/Maraudr.MCP.Server
RUN dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app/mcp-server

# Utiliser l'image SDK pour l'étape finale
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS final
WORKDIR /app


# Copier l'application principale et le serveur MCP
COPY --from=build /app .
COPY --from=build /app/mcp-server ./mcp-server

# Configurer les variables d'environnement pour le client MCP
ENV McpClient__WorkingDirectory=/app/mcp-server
ENV McpClient__Command=dotnet
ENV McpClient__Arguments__0=Maraudr.MCP.Server.dll

# Vérifier que les fichiers existent
RUN ls -la /app/mcp-server && \
    if [ ! -f /app/mcp-server/Maraudr.MCP.Server.dll ]; then echo "ERREUR: Maraudr.MCP.Server.dll introuvable"; exit 1; fi

USER $APP_UID
ENTRYPOINT ["dotnet", "Maraudr.MCP.Endpoints.dll"]