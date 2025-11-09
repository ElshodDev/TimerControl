# Base image (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

# Build image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Nusxalash va restore
COPY TimerUI/TimerUI.csproj TimerUI/
RUN dotnet restore TimerUI/TimerUI.csproj

# Boshqa source fayllarni nusxalash
COPY TimerUI/. TimerUI/
WORKDIR /src/TimerUI

# Build
RUN dotnet build TimerUI.csproj -c Release -o /app/build

# Publish
RUN dotnet publish TimerUI.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TimerUI.dll"]
