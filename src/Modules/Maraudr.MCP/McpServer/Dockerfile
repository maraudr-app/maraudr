FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy everything first to see the structure
COPY . .

# List files to debug (remove this line after fixing)

# Find and restore the project
RUN dotnet restore "McpServer.csproj"

# Build the project
RUN dotnet build "McpServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "McpServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Install curl and other dependencies needed for Ollama
RUN apt-get update && apt-get install -y \
    curl \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/*

# Install Ollama
RUN curl -fsSL https://ollama.com/install.sh | sh

# Copy the published .NET application
COPY --from=publish /app/publish .

# Create startup script
RUN echo '#!/bin/bash\n\
set -e\n\
\n\
# Start Ollama service in background\n\
ollama serve &\n\
OLLAMA_PID=$!\n\
\n\
# Wait for Ollama to be ready\n\
echo "Waiting for Ollama to start..."\n\
while ! curl -s http://localhost:11434/api/version > /dev/null; do\n\
    sleep 1\n\
done\n\
echo "Ollama is ready!"\n\
\n\
# Pull gemma:2b model\n\
echo "Pulling gemma:2b model..."\n\
ollama pull gemma:2b\n\
echo "Model pulled successfully!"\n\
\n\
# Start the MCP server\n\
echo "Starting MCP Server..."\n\
exec dotnet McpServer.dll --stdio\n\
' > /app/start.sh && chmod +x /app/start.sh

# Expose Ollama port (optional, for debugging)
EXPOSE 11434

# Use the startup script as entrypoint
ENTRYPOINT ["/app/start.sh"]