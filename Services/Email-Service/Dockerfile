#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Email-Service/Email-Service.csproj", "Services/Email-Service/"]
COPY ["Libraries/DataLayer/DataLayer.csproj", "Libraries/DataLayer/"]
COPY ["Libraries/Common/Common.csproj", "Libraries/Common/"]
COPY ["Libraries/Models/Models.csproj", "Libraries/Models/"]
RUN dotnet restore "./Services/Email-Service/Email-Service.csproj"
COPY . .
WORKDIR "/src/Services/Email-Service"
RUN dotnet build "./Email-Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Email-Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Email-Service.dll"]