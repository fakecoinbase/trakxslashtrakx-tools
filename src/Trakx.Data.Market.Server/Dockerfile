FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Trakx.Data.Market.Server/Trakx.Data.Market.Server.csproj", "Trakx.Data.Market.Server/"]
RUN dotnet restore "Trakx.Data.Market.Server/Trakx.Data.Market.Server.csproj"
COPY . .
WORKDIR "/src/Trakx.Data.Market.Server"
RUN dotnet build "Trakx.Data.Market.Server.csproj" -c Release -o /app/build -v d

FROM build AS publish
RUN dotnet publish "Trakx.Data.Market.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://+:5000
ENTRYPOINT ["dotnet", "Trakx.Data.Market.Server.dll"]