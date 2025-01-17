FROM mcr.microsoft.com/dotnet/sdk:8.0

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN


WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./ContractsApi/ContractsApi.csproj ./
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ContractsApi.csproj

# Copy everything else and build
COPY ./ContractsApi ./
RUN dotnet build -c Release -o out
RUN dotnet publish -c Release -o out

ENV ASPNETCORE_URLS=http://+:${PORT:-5000}
EXPOSE ${PORT:-5000}

CMD ASPNETCORE_URLS=http://+:${PORT:-5000} dotnet ./out/ContractsApi.dll
