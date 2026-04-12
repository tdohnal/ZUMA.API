# 1. Změna na aspnet image (nutné pro API)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# Dočasně přepneme na root, abychom mohli instalovat knihovny
USER root
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
# Vrátíme zpět uživatele pro bezpečnost
USER $APP_UID
WORKDIR /app

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GH_TOKEN

WORKDIR /src

# NuGet konfigurace
RUN dotnet nuget add source "https://nuget.pkg.github.com/tdohnal/index.json" \
    --name "github_final" \
    --username "tdohnal" \
    --password "${GH_TOKEN}" \
    --store-password-in-clear-text

COPY ["ZUMA.CustomerService/ZUMA.CustomerService.csproj", "ZUMA.CustomerService/"]
RUN dotnet restore "./ZUMA.CustomerService/ZUMA.CustomerService.csproj"

COPY . .
WORKDIR "/src/ZUMA.CustomerService"
RUN dotnet build "./ZUMA.CustomerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ZUMA.CustomerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZUMA.CustomerService.dll"]