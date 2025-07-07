FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS final

WORKDIR /source

# Copy all sources for dotnet run
COPY . .

WORKDIR /source/Maraudr.MCP.Endpoints

ENTRYPOINT ["dotnet", "run", "--project", "/source/Maraudr.MCP.Endpoints"]
