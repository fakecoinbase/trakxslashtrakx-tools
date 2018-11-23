FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 51131
EXPOSE 44316

FROM microsoft/dotnet:2.1-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY Trakx.MarketData.Feeds/Trakx.MarketData.Feeds.csproj Trakx.MarketData.Feeds/
RUN dotnet restore Trakx.MarketData.Feeds/Trakx.MarketData.Feeds.csproj
COPY . .
WORKDIR /src/Trakx.MarketData.Feeds
RUN dotnet build Trakx.MarketData.Feeds.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Trakx.MarketData.Feeds.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Trakx.MarketData.Feeds.dll"]
