# syntax=docker/dockerfile:1.4
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG MESSAGING_CONTRACTS_VERSION=1.0.96-alpha
ARG SERVICE_DEFAULTS_VERSION=1.0.89-alpha
WORKDIR /src

COPY ["Maliev.LifecycleService.Api/Maliev.LifecycleService.Api.csproj", "Maliev.LifecycleService.Api/"]
COPY ["Maliev.LifecycleService.Application/Maliev.LifecycleService.Application.csproj", "Maliev.LifecycleService.Application/"]
COPY ["Maliev.LifecycleService.Domain/Maliev.LifecycleService.Domain.csproj", "Maliev.LifecycleService.Domain/"]
COPY ["Maliev.LifecycleService.Infrastructure/Maliev.LifecycleService.Infrastructure.csproj", "Maliev.LifecycleService.Infrastructure/"]
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]

RUN --mount=type=secret,id=nuget_username \
    --mount=type=secret,id=nuget_password \
    GITHUB_ACTIONS=true \
    NUGET_USERNAME=$(cat /run/secrets/nuget_username) \
    NUGET_PASSWORD=$(cat /run/secrets/nuget_password) \
    dotnet restore "./Maliev.LifecycleService.Api/Maliev.LifecycleService.Api.csproj" \
      -p:MessagingContractsVersion=$MESSAGING_CONTRACTS_VERSION \
      -p:ServiceDefaultsVersion=$SERVICE_DEFAULTS_VERSION

COPY . .
WORKDIR "/src/Maliev.LifecycleService.Api"
RUN GITHUB_ACTIONS=true dotnet build "./Maliev.LifecycleService.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore \
    -p:MessagingContractsVersion=$MESSAGING_CONTRACTS_VERSION \
    -p:ServiceDefaultsVersion=$SERVICE_DEFAULTS_VERSION

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG MESSAGING_CONTRACTS_VERSION=1.0.96-alpha
ARG SERVICE_DEFAULTS_VERSION=1.0.89-alpha
RUN GITHUB_ACTIONS=true dotnet publish "./Maliev.LifecycleService.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false \
    -p:MessagingContractsVersion=$MESSAGING_CONTRACTS_VERSION \
    -p:ServiceDefaultsVersion=$SERVICE_DEFAULTS_VERSION

FROM base AS final
WORKDIR /app
RUN chown -R $APP_UID:$APP_UID /app
USER $APP_UID
COPY --chown=$APP_UID:$APP_UID --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Maliev.LifecycleService.Api.dll"]
