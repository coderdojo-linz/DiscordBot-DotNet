FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
COPY . ./

# Copy csproj and restore as distinct layers
WORKDIR /app/DiscordBot_Template
RUN dotnet restore DiscordBot.csproj

# Copy everything else and build
RUN dotnet publish DiscordBot.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build-env /app/DiscordBot_Template/out .
ENTRYPOINT ["dotnet", "DiscordBot.dll"]
