FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

#COPY nuget.docker.config nuget.config
#COPY TempLocalFeed TempLocalFeed

COPY ["MystSafe.Backend.Relay/MystSafe.Backend.Relay.csproj", "MystSafe.Backend.Relay/"]
COPY ["MystSafe.Backend.DB/MystSafe.Backend.DB.csproj", "MystSafe.Backend.DB/"]
COPY ["MystSafe.Shared.Common/MystSafe.Shared.Common.csproj", "MystSafe.Shared.Common/"]
COPY ["MystSafe.Shared.Crypto/MystSafe.Shared.Crypto.csproj", "MystSafe.Shared.Crypto/"]
COPY ["MystSafe.Shared.CryptoLicense/MystSafe.Shared.CryptoLicense.csproj", "MystSafe.Shared.CryptoLicense/"]

COPY LocalPackages ./LocalPackages
COPY NuGet.Config ./

RUN dotnet restore "MystSafe.Backend.Relay/MystSafe.Backend.Relay.csproj"
#RUN dotnet restore "MystSafe.Backend.Relay/MystSafe.Backend.Relay.csproj" --verbosity detailed

COPY . .
WORKDIR "/src/MystSafe.Backend.Relay"
RUN dotnet build "MystSafe.Backend.Relay.csproj" -c Release -o /app/build /p:GeneratePackageOnBuild=false --no-restore /maxcpucount

FROM build AS publish
RUN dotnet publish "MystSafe.Backend.Relay.csproj" -c Release /p:PublishTrimmed=false -o /app/publish /p:UseAppHost=false /p:GeneratePackageOnBuild=false

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MystSafe.Backend.Relay.dll"]
