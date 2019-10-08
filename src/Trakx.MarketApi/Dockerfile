FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Trakx.MarketApi/Trakx.MarketApi.csproj", "Trakx.MarketApi/"]
RUN dotnet restore "Trakx.MarketApi/Trakx.MarketApi.csproj"
COPY . .
WORKDIR "/src/Trakx.MarketApi"
RUN dotnet build "Trakx.MarketApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Trakx.MarketApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Trakx.MarketApi.dll"]