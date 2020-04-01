[![Build status](https://dev.azure.com/trakx-io/data-market/_apis/build/status/data-market-ASP.NET%20Core-CI)](https://dev.azure.com/trakx-io/data-market/_build/latest?definitionId=1)

# Trakx data-market
A small web service providing info about Trakx indices and usefull live calculations.
Data sources used for the calculations are currently CoinGecko and CryptoCompare.

# Running version
A running version can be found at preprod.marketdata.trakx.io

# Avoid committing you secrets and keys
In order to be able to run some integration tests, the file 
src/Trakx.Data.Tests/Tools/Secrets.cs should be modified to include your own api keys and other secrets, 
to avoid accidental commits to GitHub, it is a good idea to ignore local changes to that file using by running:
```
git update-index --skip-worktree src/Trakx.Data.Tests/Tools/Secrets.cs
```
