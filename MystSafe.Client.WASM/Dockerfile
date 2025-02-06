FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Replicate Node.js installation in the build stage
RUN apt-get update && \
    apt-get install -y curl && \
    curl -sL https://deb.nodesource.com/setup_21.x | bash - && \
    apt-get install -y nodejs 

COPY nuget.docker.config nuget.config
COPY TempLocalFeed TempLocalFeed

COPY ["MystSafe.Client.WASM/MystSafe.Client.App.csproj", "MystSafe.Client.WASM/"]
COPY ["MystSafe.Client.Fido2.BlazorWebAssembly/MystSafe.Client.Fido2.BlazorWebAssembly.csproj", "MystSafe.Client.Fido2.BlazorWebAssembly/"]
COPY ["MystSafe.Client.Fido2.Models/MystSafe.Client.Fido2.Models.csproj", "MystSafe.Client.Fido2.Models/"]
COPY ["MystSafe.Client.Fido2/MystSafe.Client.Fido2.csproj", "MystSafe.Client.Fido2/"]
COPY ["MystSafe.Client.Engine/MystSafe.Client.Engine.csproj", "MystSafe.Client.Engine/"]
COPY ["MystSafe.Client.Base/MystSafe.Client.Base.csproj", "MystSafe.Client.Base/"]
RUN dotnet restore "MystSafe.Client.WASM/MystSafe.Client.App.csproj"

COPY . .
WORKDIR "/src/MystSafe.Client.WASM"
RUN dotnet build "MystSafe.Client.App.csproj" -c $BUILD_CONFIGURATION -o /app/build
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MystSafe.Client.App.csproj" -c $BUILD_CONFIGURATION /p:PublishTrimmed=false -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
COPY --from=publish /app/publish /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]