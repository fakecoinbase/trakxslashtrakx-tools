FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 6000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ./src .
RUN dotnet restore "Trakx.IndiceManager.Client/Trakx.IndiceManager.Client.csproj"

WORKDIR /src/Trakx.IndiceManager.Client
RUN dotnet build "Trakx.IndiceManager.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Trakx.IndiceManager.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://+:6000
ENTRYPOINT ["dotnet", "Trakx.IndiceManager.Client.dll"]
