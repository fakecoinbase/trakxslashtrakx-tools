[![Build Status](https://dev.azure.com/trakx-io/index-manager/_apis/build/status/data-market-ASP.NET%20Core-CI?branchName=dev)](https://dev.azure.com/trakx-io/index-manager/_build/latest?definitionId=1&branchName=dev)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/fb9de3c044504d1abd8994d4c38819d8)](https://www.codacy.com/gh/trakx/index-manager?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=trakx/index-manager&amp;utm_campaign=Badge_Grade)
[![Coverage Status](https://coveralls.io/repos/github/trakx/index-manager/badge.svg)](https://coveralls.io/github/trakx/index-manager)



# Trakx index-manager
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
