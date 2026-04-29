# Tato fáze je pro běh aplikace
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# Tato fáze staví aplikaci
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release
ARG GH_TOKEN

WORKDIR /src

# 1. Zkopírujeme jen csproj (zatím bez nuget.config)
COPY ["ZUMA.API/ZUMA.API.csproj", "ZUMA.API/"]

# 2. KOMPLETNÍ CLEAN: Smažeme výchozí configy v Dockeru a přidáme jen to, co chceme
# Tím zajistíme, že tam nebude žádná špatná URL z tvého nuget.config
RUN dotnet nuget add source "https://nuget.pkg.github.com/tdohnal/index.json" \
    --name "github_final" \
    --username "tdohnal" \
    --password "${GH_TOKEN}" \
    --store-password-in-clear-text

# 3. Restore - teď už to musí brát z toho nového zdroje "github_final"
RUN dotnet restore "ZUMA.API/ZUMA.API.csproj"

# 4. Zkopírujeme zbytek a stavíme
COPY . .
WORKDIR "/src/ZUMA.API"
RUN dotnet build "ZUMA.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish

ARG APP_VERSION=1.0.0
RUN dotnet publish "ZUMA.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:Version=$APP_VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZUMA.API.dll"]