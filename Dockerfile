FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder
WORKDIR /src

COPY ./Sources/AutoPayout.Result ./AutoPayout.Result
COPY ./Sources/AutoPayout.Sportsbook ./AutoPayout.Sportsbook
COPY ./Sources/Fanex.AutoPayout.Service/Fanex.AutoPayout.Service.csproj ./Fanex.AutoPayout.Service/Fanex.AutoPayout.Service.csproj

WORKDIR /src/Fanex.AutoPayout.Service
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore -v n

COPY ./Sources/Fanex.AutoPayout.Service .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine AS base

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "Fanex.AutoPayout.Service.dll"]
