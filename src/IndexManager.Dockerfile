FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 4000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
RUN apt-get update -y
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash --debug
RUN apt-get install nodejs -yq
WORKDIR /src
COPY ./src .
RUN dotnet restore "Trakx.IndexManager.Server/Trakx.IndexManager.Server.csproj"

WORKDIR "/src/Trakx.IndexManager.Server"
RUN dotnet build "Trakx.IndexManager.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Trakx.IndexManager.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://+:4000
ENV MESSARI_API_KEY "replace me"
ENV INFURA_API_KEY "replace me"
ENTRYPOINT ["dotnet", "Trakx.IndexManager.Server.dll"]
