FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./ContractsApi.sln ./
COPY ./ContractsApi/ContractsApi.csproj ./ContractsApi/
COPY ./ContractsApi.Tests/ContractsApi.Tests.csproj ./ContractsApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ContractsApi/ContractsApi.csproj
RUN dotnet restore ./ContractsApi.Tests/ContractsApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out ContractsApi.Tests/ContractsApi.Tests.csproj

CMD dotnet test
