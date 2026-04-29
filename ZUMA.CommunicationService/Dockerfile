# --- Runtime stage (Tady to musí být!) ---
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
# Přepneme na roota, abychom mohli instalovat
USER root 
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
# Přepneme zpět na bezpečný UID aplikace
USER $APP_UID
WORKDIR /app

# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GH_TOKEN

WORKDIR /src

# Tady v buildu to teoreticky nepotřebuješ, pokud tam nespouštíš testy proti DB
RUN dotnet nuget add source "https://nuget.pkg.github.com/tdohnal/index.json" \
    --name "github_final" \
    --username "tdohnal" \
    --password "${GH_TOKEN}" \
    --store-password-in-clear-text

COPY ["ZUMA.CommunicationService/ZUMA.CommunicationService.csproj", "ZUMA.CommunicationService/"]
RUN dotnet restore "./ZUMA.CommunicationService/ZUMA.CommunicationService.csproj"

COPY . .
WORKDIR "/src/ZUMA.CommunicationService"
RUN dotnet build "./ZUMA.CommunicationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- Publish stage ---
FROM build AS publish

ARG APP_VERSION=1.0.0
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ZUMA.CommunicationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:Version=$APP_VERSION

# --- Final stage ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZUMA.CommunicationService.dll"]