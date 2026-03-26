# Stage 1: Runtime (jen pro spuštění)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

# Stage 2: Build (tady se to kompiluje)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Tady kopírujeme jen to, co EmailService potřebuje
COPY ["ZUMA.EmailService/ZUMA.EmailService.csproj", "ZUMA.EmailService/"]
# Pokud EmailService využívá tvou BussinessLogic, přidej i tenhle řádek:
COPY ["ZUMA.BussinessLogic/ZUMA.BussinessLogic.csproj", "ZUMA.BussinessLogic/"]

RUN dotnet restore "./ZUMA.EmailService/ZUMA.EmailService.csproj"
COPY . .
WORKDIR "/src/ZUMA.EmailService"
RUN dotnet build "./ZUMA.EmailService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ZUMA.EmailService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Finální obraz
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZUMA.EmailService.dll"]