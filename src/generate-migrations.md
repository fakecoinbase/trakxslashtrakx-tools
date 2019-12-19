try, from console
```
dotnet ef migrations add "CreateIndexRepository" --context IndexRepositoryContext --project src\Trakx.Data.Models.csproj --startup-project src\Trakx.Data.Market.Server.csproj
```

or, from package manager console
```
add-migration "CreateIndexRepository" -project Trakx.Data.Models -startupProject Trakx.Data.Market.Server -Context IndexRepositoryContext
```
