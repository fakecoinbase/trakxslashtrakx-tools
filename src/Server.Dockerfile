FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
RUN apt-get update -y
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash --debug
RUN apt-get install nodejs -yq
WORKDIR /src
COPY ./src .
RUN dotnet restore "Trakx.Data.Market.Server/Trakx.Data.Market.Server.csproj"

WORKDIR "/src/Trakx.Data.Market.Server"
RUN dotnet build "Trakx.Data.Market.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Trakx.Data.Market.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://+:5000
ENV MESSARI_API_KEY "replace me"
ENV INFURA_API_KEY "replace me"
ENTRYPOINT ["dotnet", "Trakx.Data.Market.Server.dll"]