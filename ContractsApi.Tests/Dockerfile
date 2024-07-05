FROM mcr.microsoft.com/dotnet/sdk:6.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

ARG SONAR_TOKEN
ENV SONAR_TOKEN=$SONAR_TOKEN

WORKDIR /app

# Install and run sonar cloud scanner
# RUN apt-get update && apt-get install -y openjdk17
RUN apt-get update && apt-get install -y openjdk-17-jdk

# RUN apk update \
#  && apk upgrade --no-cache \
#  && apk add --no-cache openjdk-17-jdk

RUN dotnet tool install --global dotnet-sonarscanner
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet sonarscanner begin /k:"LBHackney-IT_contracts-api" /o:"lbhackney-it" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="${SONAR_TOKEN}"

# Copy csproj and nuget config and restore as distinct layers
COPY ./ContractsApi.sln ./
COPY ./ContractsApi/ContractsApi.csproj ./ContractsApi/
COPY ./ContractsApi.Tests/ContractsApi.Tests.csproj ./ContractsApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ContractsApi/ContractsApi.csproj
RUN dotnet restore ./ContractsApi.Tests/ContractsApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c Release -o out ContractsApi/ContractsApi.csproj
RUN dotnet build -c debug -o out ContractsApi.Tests/ContractsApi.Tests.csproj

CMD dotnet test

RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"