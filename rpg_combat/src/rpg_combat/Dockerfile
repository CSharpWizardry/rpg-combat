
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["rpg_combat/rpg-combat.csproj", "rpg_combat/"]
RUN dotnet restore "rpg_combat/rpg-combat.csproj"
COPY . .
WORKDIR "/src/rpg_combat"
RUN dotnet build "rpg-combat.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "rpg-combat.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "rpg-combat.dll"]


# syntax=docker/dockerfile:1
# FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
# WORKDIR /app
# EXPOSE 5001
# EXPOSE 5000
# EXPOSE 8080

# # Copy csproj and restore as distinct layers
# COPY *.csproj ./
# RUN dotnet restore

# # Copy everything else and build
# COPY . .
# RUN dotnet publish -c Release -o out

# # Build runtime image
# FROM mcr.microsoft.com/dotnet/aspnet:3.1
# WORKDIR /app
# COPY --from=build-env /app/out .
# ENTRYPOINT ["dotnet", "rpg-combat.dll"]