﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y libgdiplus
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CarpetPlanner/CarpetPlanner.csproj", "CarpetPlanner/"]
RUN dotnet restore "CarpetPlanner/CarpetPlanner.csproj"
COPY . .
WORKDIR "/src/CarpetPlanner"
RUN dotnet build "CarpetPlanner.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM --platform=$BUILDPLATFORM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CarpetPlanner.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarpetPlanner.dll"]
