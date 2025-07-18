FROM mcr.microsoft.com/dotnet/sdk:8.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

ARG SONAR_TOKEN
ENV SONAR_TOKEN=$SONAR_TOKEN

WORKDIR /app

# Install and run sonar cloud scanner
RUN apt-get update && apt-get install -y openjdk-17-jdk

RUN dotnet tool install --global dotnet-sonarscanner

# Install report generator
RUN dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.2.0

ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet sonarscanner begin /k:"LBHackney-IT_contracts-api" /o:"lbhackney-it" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="${SONAR_TOKEN}" /d:sonar.coverageReportPaths="coverage/SonarQube.xml" /d:sonar.dotnet.excludeTestProjects=true /d:sonar.exclusions="**/*.js, **/*.ts, **/*.css"

# Copy csproj and nuget config and restore as distinct layers
COPY ./ContractsApi.sln ./
COPY ./ContractsApi/ContractsApi.csproj ./ContractsApi/
COPY ./ContractsApi.Tests/ContractsApi.Tests.csproj ./ContractsApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ContractsApi/ContractsApi.csproj
RUN dotnet restore ./ContractsApi.Tests/ContractsApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build ./ContractsApi/ContractsApi.csproj --no-restore --no-dependencies
RUN dotnet build ./ContractsApi.Tests/ContractsApi.Tests.csproj --no-restore

CMD dotnet test ./ContractsApi.Tests/ContractsApi.Tests.csproj --no-build --collect:"XPlat Code Coverage" --results-directory ./coverage \
    && reportgenerator "-reports:./coverage/*/coverage.cobertura.xml" "-targetdir:coverage" "-reporttypes:SonarQube" "-verbosity:Off" \
    && (dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}" > /dev/null)
