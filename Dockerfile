# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CM_API/CM_API.csproj", "CM_API/"]
COPY ["Services/CM.ApplicationService/CM.ApplicationService.csproj", "Services/CM.ApplicationService/"]
COPY ["Services/CM.Dtos/CM.Dtos.csproj", "Services/CM.Dtos/"]
COPY ["Services/CM.Domain/CM.Domain.csproj", "Services/CM.Domain/"]
COPY ["Services/Shared/Share.ApplicationService/Share.ApplicationService.csproj", "Services/Shared/Share.ApplicationService/"]
COPY ["Services/CM.Infrastructure/CM.Infrastructure.csproj", "Services/CM.Infrastructure/"]
COPY ["Services/Shared/Share.Constant/Share.Constant.csproj", "Services/Shared/Share.Constant/"]
RUN dotnet restore "./CM_API/CM_API.csproj"
COPY . .
WORKDIR "/src/CM_API"
RUN dotnet build "./CM_API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CM_API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CM_API.dll"]
