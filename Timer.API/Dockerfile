# Base image (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Nusxalash va restore
COPY Timer.API/*.csproj Timer.API/
RUN dotnet restore Timer.API/Timer.API.csproj

# Source fayllarni nusxalash
COPY Timer.API/. Timer.API/
WORKDIR /src/Timer.API

# Build
RUN dotnet build Timer.API.csproj -c Release -o /app/build

# Publish
RUN dotnet publish Timer.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Timer.API.dll"]
