# STEP 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopíruj solution a celý složkový strom Eshop_MK
COPY ZUMA.API.sln ./
COPY ZUMA.API/ZUMA.API/ ./ZUMA.API/ZUMA.API/
COPY ZUMA.API/ZUMA.API.Client/ ./Eshop_MK/Eshop_MK.Client/

# Restore a publish
WORKDIR /src/ZUMA.API/ZUMA.API
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# STEP 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ZUMA.API.dll"]
