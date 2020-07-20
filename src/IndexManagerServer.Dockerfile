FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 4000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ./src .
RUN dotnet restore "Trakx.IndiceManager.Server/Trakx.IndiceManager.Server.csproj"

WORKDIR /src/Trakx.IndiceManager.Server
RUN dotnet build "Trakx.IndiceManager.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Trakx.IndiceManager.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://+:4000
ENV MESSARI_API_KEY "replace me"
ENV INFURA_API_KEY "replace me"
ENV COINBASE_PASSPHRASE_KEY "replace me"
ENV COINBASE_API_KEY "replace me"
ENTRYPOINT ["dotnet", "Trakx.IndiceManager.Server.dll"]
