#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
ENV Connection=mongodb://172.17.0.1:27017
ENV DatabaseName=UsersDatabase
ENV UserCollectionName=Users
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/API.csproj", "API/"]
COPY ["Libraries/Common/Common.csproj", "Libraries/Common/"]
COPY ["Libraries/DataLayer/DataLayer.csproj", "Libraries/DataLayer/"]
COPY ["Libraries/Models/Models.csproj", "Libraries/Models/"]
COPY ["Libraries/Payments/Payments.csproj", "Libraries/Payments/"]
COPY ["Libraries/Validation/Validation.csproj", "Libraries/Validation/"]
RUN dotnet restore "./API/API.csproj"
COPY . .
WORKDIR "/src/API"
RUN dotnet build "./API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]