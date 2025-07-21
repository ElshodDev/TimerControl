# Base image (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

# Build image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TimerUI/TimerUI.csproj", "TimerUI/"]
RUN dotnet restore "TimerUI/TimerUI.csproj"
COPY . .
WORKDIR "/src/TimerUI"
RUN dotnet build "TimerUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TimerUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimerUI.dll"]
