# Tato fáze je pro běh aplikace
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# Tato fáze staví aplikaci
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

# --- TADY JE TA ZMĚNA ---
# Musíš ARG definovat uvnitř fáze, kde se používá (build)
ARG GH_TOKEN
# Nastavíme ji jako ENV, aby ji viděl proces dotnet restore
ENV GH_TOKEN=$GH_TOKEN
# ------------------------

WORKDIR /src
# Nejdřív kopírujeme config a csproj
COPY ["nuget.config", "."]
COPY ["ZUMA.API/ZUMA.API.csproj", "ZUMA.API/"]

# Teď už dotnet restore uvidí tvůj token z ENV
RUN dotnet restore "./ZUMA.API/ZUMA.API.csproj" --configfile nuget.config

COPY . .
WORKDIR "/src/ZUMA.API"
RUN dotnet build "./ZUMA.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ZUMA.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZUMA.API.dll"]