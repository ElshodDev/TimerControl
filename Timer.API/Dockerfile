# Base image (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Timer.API/Timer.API.csproj", "Timer.API/"]
RUN dotnet restore "Timer.API/Timer.API.csproj"
COPY . .
WORKDIR "/src/Timer.API"
RUN dotnet build "Timer.API.csproj" -c Release -o /app/build

# Publish image
FROM build AS publish
RUN dotnet publish "Timer.API.csproj" -c Release -o /app/publish --no-restore

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Timer.API.dll"]
