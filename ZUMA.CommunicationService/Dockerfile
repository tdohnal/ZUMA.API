# Stage 1: Runtime (jen pro spuštění)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

# Stage 2: Build (tady se to kompiluje)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Tady kopírujeme jen to, co EmailService potřebuje
COPY ["ZUMA.CommunicationService/ZUMA.CommunicationService.csproj", "ZUMA.CommunicationService/"]
RUN dotnet restore "./ZUMA.CommunicationService/ZUMA.CommunicationService.csproj"
COPY . .
WORKDIR "/src/ZUMA.CommunicationService"
RUN dotnet build "./ZUMA.CommunicationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ZUMA.CommunicationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Finální obraz
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZUMA.CommunicationService.dll"]