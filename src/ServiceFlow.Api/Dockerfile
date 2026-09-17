# 1. Build stage: compile C# code using .NET 10 SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy project files first (leverages Docker layer caching for fast restore)
COPY src/ServiceFlow.Core/ServiceFlow.Core.csproj src/ServiceFlow.Core/
COPY src/ServiceFlow.Infrastructure/ServiceFlow.Infrastructure.csproj src/ServiceFlow.Infrastructure/
COPY src/ServiceFlow.Api/ServiceFlow.Api.csproj src/ServiceFlow.Api/
RUN dotnet restore src/ServiceFlow.Api/ServiceFlow.Api.csproj

# Copy remaining source code and publish release build
COPY src/ src/
RUN dotnet publish src/ServiceFlow.Api/ServiceFlow.Api.csproj -c Release -o /app/publish

# 2. Runtime stage: lightweight production image with .NET 10 runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5021
ENV ASPNETCORE_URLS=http://+:5021

ENTRYPOINT ["dotnet", "ServiceFlow.Api.dll"]

