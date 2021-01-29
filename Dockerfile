# FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build-env
WORKDIR /app
COPY . ./

# Copy csproj and restore as distinct layers
WORKDIR /app/DiscordBot
RUN dotnet restore DiscordBot.csproj

# Copy everything else and build
RUN dotnet publish DiscordBot.csproj -c Release -o out

# Build runtime image
# FROM mcr.microsoft.com/dotnet/core/runtime:3.1
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build-env /app/DiscordBot/out .
ENTRYPOINT ["dotnet", "DiscordBot.dll"]
