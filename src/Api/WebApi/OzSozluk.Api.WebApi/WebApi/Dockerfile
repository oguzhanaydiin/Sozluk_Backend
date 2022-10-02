#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Api/WebApi/OzSozluk.Api.WebApi/WebApi/WebApi.csproj", "src/Api/WebApi/OzSozluk.Api.WebApi/WebApi/"]
COPY ["src/Api/Core/OzSozluk.Api.Application/Application/Application.csproj", "src/Api/Core/OzSozluk.Api.Application/Application/"]
COPY ["src/Common/OzSozluk.Common/Common/Common.csproj", "src/Common/OzSozluk.Common/Common/"]
COPY ["src/Api/Core/OzSozluk.Api.Domain/Domain/Domain.csproj", "src/Api/Core/OzSozluk.Api.Domain/Domain/"]
COPY ["src/Api/Infrastructure/OzSozluk.Infrastructure.Persistence/Persistence/Persistence.csproj", "src/Api/Infrastructure/OzSozluk.Infrastructure.Persistence/Persistence/"]
RUN dotnet restore "src/Api/WebApi/OzSozluk.Api.WebApi/WebApi/WebApi.csproj"
COPY . .
WORKDIR "/src/src/Api/WebApi/OzSozluk.Api.WebApi/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]