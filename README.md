[![Build Status](https://dev.azure.com/trakx-io/index-manager/_apis/build/status/data-market-ASP.NET%20Core-CI?branchName=dev)](https://dev.azure.com/trakx-io/index-manager/_build/latest?definitionId=1&branchName=dev)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/fb9de3c044504d1abd8994d4c38819d8)](https://www.codacy.com/gh/trakx/trakx-tools?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=trakx/trakx-tools&amp;utm_campaign=Badge_Grade)
[![Coverage Status](https://coveralls.io/repos/github/trakx/trakx-tools/badge.svg?branch=dev)](https://coveralls.io/github/trakx/trakx-tools?branch=dev)



# Trakx Tools
A solution grouping several tools used at Trakx for creation, rebalancing, and pricing of indices.

## Market Data Server Running version
A running version can be found at preprod.marketdata.trakx.io

## Avoid committing you secrets and keys
In order to be able to run some integration tests, the file 
src/Trakx.Tests/Tools/Secrets.cs should be modified to include your own api keys and other secrets, 
to avoid accidental commits to GitHub, it is a good idea to ignore local changes to that file using by running:
```
git update-index --skip-worktree src/Trakx.Tests/Tools/Secrets.cs
```
