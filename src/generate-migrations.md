start the container with something like 
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=oh_no_this-needs_toChange4Real" -p 1533:1433 -v sqlvolume:/var/opt/mssql --name indice-repository -d mcr.microsoft.com/mssql/server

try, from console
```
dotnet ef migrations add "CreateIndiceRepository" --context IndiceRepositoryContext --project src\Trakx.Data.Models.csproj --startup-project src\Trakx.Data.Market.Server.csproj
```

or, from package manager console
// docker start "7c9b9d8565559d803b3fa07413c69b4aa6935b7d902179806d4aa8b26007f272"
```
add-migration "CreateIndiceRepository" -project Trakx.Persistence -startupProject Trakx.MarketData.Server -Context IndiceRepositoryContext
```
