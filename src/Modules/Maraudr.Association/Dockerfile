# syntax=docker/dockerfile:1

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

COPY . /source

WORKDIR /source/Maraudr.Associations.Endpoints

ARG TARGETARCH

RUN dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "Maraudr.Associations.Endpoints.dll"]
