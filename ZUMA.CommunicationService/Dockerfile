# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

# DŮLEŽITÉ: Definice argumentu uvnitř buildu
ARG GH_TOKEN

WORKDIR /src

RUN dotnet nuget add source "https://nuget.pkg.github.com/tdohnal/index.json" \
    --name "github_final" \
    --username "tdohnal" \
    --PASS "${GH_TOKEN}" \
    --store-PASS-in-clear-text

COPY ["ZUMA.CommunicationService/ZUMA.CommunicationService.csproj", "ZUMA.CommunicationService/"]

# Restore s použitím tvého tokenu z ENV
RUN dotnet restore "./ZUMA.CommunicationService/ZUMA.CommunicationService.csproj"

COPY . .
WORKDIR "/src/ZUMA.CommunicationService"
RUN dotnet build "./ZUMA.CommunicationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- Publish stage ---
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ZUMA.CommunicationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- Final stage ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZUMA.CommunicationService.dll"]