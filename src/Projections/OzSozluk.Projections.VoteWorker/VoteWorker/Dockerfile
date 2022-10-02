#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Projections/OzSozluk.Projections.VoteWorker/VoteWorker/VoteWorker.csproj", "src/Projections/OzSozluk.Projections.VoteWorker/VoteWorker/"]
COPY ["src/Common/OzSozluk.Common/Common/Common.csproj", "src/Common/OzSozluk.Common/Common/"]
RUN dotnet restore "src/Projections/OzSozluk.Projections.VoteWorker/VoteWorker/VoteWorker.csproj"
COPY . .
WORKDIR "/src/src/Projections/OzSozluk.Projections.VoteWorker/VoteWorker"
RUN dotnet build "VoteWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VoteWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VoteWorker.dll"]