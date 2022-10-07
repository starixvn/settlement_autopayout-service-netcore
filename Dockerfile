FROM hub.nexdev.net/base/dotnet/core/sdk:6.0 AS builder
WORKDIR /src

COPY ./Sources/AutoPayout.Result ./AutoPayout.Result
COPY ./Sources/AutoPayout.Sportsbook ./AutoPayout.Sportsbook
COPY ./Sources/Fanex.AutoPayout.Service/Fanex.AutoPayout.Service.csproj ./Fanex.AutoPayout.Service/Fanex.AutoPayout.Service.csproj

WORKDIR /src/Fanex.AutoPayout.Service
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN dotnet restore -v n

COPY ./Sources/Fanex.AutoPayout.Service .

RUN dotnet publish -r linux-x64 --self-contained false -o /app 

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0 AS base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "Fanex.AutoPayout.Service.dll"]
