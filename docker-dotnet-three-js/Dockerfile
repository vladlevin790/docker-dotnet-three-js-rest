FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["docker-dotnet-three-js/docker-dotnet-three-js.csproj", "docker-dotnet-three-js/"]
RUN dotnet restore "docker-dotnet-three-js/docker-dotnet-three-js.csproj"
COPY . .
WORKDIR "/src/docker-dotnet-three-js"
RUN dotnet build "docker-dotnet-three-js.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "docker-dotnet-three-js.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "docker-dotnet-three-js.dll"]
