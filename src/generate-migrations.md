start the container with something like 
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=oh_no_this-needs_toChange4Real" -p 1533:1433 -v sqlvolume:/var/opt/mssql --name index-repository -d mcr.microsoft.com/mssql/server

try, from console
```
dotnet ef migrations add "CreateIndexRepository" --context IndexRepositoryContext --project src\Trakx.Data.Models.csproj --startup-project src\Trakx.Data.Market.Server.csproj
```

or, from package manager console
```
add-migration "CreateIndexRepository" -project Trakx.Data.Models -startupProject Trakx.Data.Market.Server -Context IndexRepositoryContext
```