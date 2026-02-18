FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
# Instala o cliente PostgreSQL
RUN apt-get update && apt-get install -y netcat wget gnupg lsb-release && \
    echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list && \
    wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add - && \
    apt-get update && apt-get install -y postgresql-client-15

# DevExpress Reporting dependencies
RUN apt-get update && \
    apt-get install -y \
        libgdiplus \
        libicu-dev \
        libharfbuzz0b \
        libfontconfig1 \
        libfreetype6 \
        libpango-1.0-0 \
        libpangocairo-1.0 \
        fontconfig && \
    fc-cache -f -v

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SK.Report/NuGet.config", "SK.Report/"]
COPY ["SK.Report/SK.Report.csproj", "SK.Report/"]
RUN dotnet restore "./SK.Report/SK.Report.csproj"
COPY . .
WORKDIR "/src/SK.Report"
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs
RUN npm install -g npm
RUN npm install
RUN dotnet build "./SK.Report.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SK.Report.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:OutputPath=/app/build --no-build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SK.Report.dll"]