FROM mcr.microsoft.com/dotnet/sdk:10.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

ARG SONAR_TOKEN
ENV SONAR_TOKEN=$SONAR_TOKEN

WORKDIR /app

# Install sonar scanner and coverage report generator.
# Do not pre-install Java: SonarScanner for .NET (7+) auto-provisions a JRE at analysis
# time, which keeps the runtime in step with SonarQube Cloud's requirements.
RUN dotnet tool install --global dotnet-sonarscanner \
&& dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.2.0

ENV PATH="$PATH:/root/.dotnet/tools"

RUN if [ -n "${SONAR_TOKEN}" ]; then dotnet sonarscanner begin \
/k:"LBHackney-IT_contracts-api" \
/o:"lbhackney-it" \
/d:sonar.host.url=https://sonarcloud.io \
/d:sonar.login="${SONAR_TOKEN}" \
/d:sonar.coverageReportPaths="coverage/SonarQube.xml" \
/d:sonar.dotnet.excludeTestProjects=true \
/d:sonar.exclusions="**/*.js, **/*.ts, **/*.css"; fi

# Copy csproj and nuget config and restore as distinct layers
COPY ./ContractsApi.sln ./
COPY ./ContractsApi/ContractsApi.csproj ./ContractsApi/
COPY ./ContractsApi.Tests/ContractsApi.Tests.csproj ./ContractsApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ContractsApi/ContractsApi.csproj \
&& dotnet restore ./ContractsApi.Tests/ContractsApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build ./ContractsApi/ContractsApi.csproj --no-dependencies \
&& dotnet build ./ContractsApi.Tests/ContractsApi.Tests.csproj

CMD dotnet test ./ContractsApi.Tests/ContractsApi.Tests.csproj --no-build --collect:"XPlat Code Coverage" --results-directory ./coverage \
    && reportgenerator "-reports:./coverage/*/coverage.cobertura.xml" "-targetdir:coverage" "-reporttypes:SonarQube" "-verbosity:Off" \
    && if [ -n "${SONAR_TOKEN}" ]; then (dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}" > /dev/null); fi
