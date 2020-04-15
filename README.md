
[![Build Status](https://dev.azure.com/trakx-io/trakx-tools/_apis/build/status/trakx-tools-ASP.NET%20Core-CI?branchName=dev)](https://dev.azure.com/trakx-io/trakx-tools/_build/latest?definitionId=1&branchName=dev)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/fb9de3c044504d1abd8994d4c38819d8)](https://www.codacy.com/gh/trakx/trakx-tools?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=trakx/trakx-tools&amp;utm_campaign=Badge_Grade)
[![Coverage Status](https://coveralls.io/repos/github/trakx/trakx-tools/badge.svg?branch=dev)](https://coveralls.io/github/trakx/trakx-tools?branch=dev)



# Trakx Tools
A solution grouping several tools used at Trakx for creation, rebalancing, and pricing of indices. This workspace currently hosts a couple of different but related tools being developed at Trakx.
- Trakx.Persistence relies on SQL Server to Entity to define and interact with a database where all indices are defined, and valuation will eventually be maintained.
- Trakx.MarketData.Server is an Asp.Net Core server currently used by the exchange to retrieve details about the indexes, such as composition, and live Net Asset Value.
- Trakx.Contracts mainly consists of auto-generated classes porting ABIs of existing smart contracts on .Net to allow on chain interactions via Nethereum.
- Trakx.Common groups all common interfaces and implementations of the services used by the above projects. A non negligeable part of the code are experimental uses of various third-party market data providers (CoinGecko, Messari, Kaiko, CryptoCompare, etc.) and will soon be moved to its own project.
- Trakx.Tests groups all Unit and Integration tests ensuring that the above projects don't go wrong. As they get out of experimental stage, code coverage should increase to acceptable standards.

## Market Data Server Running version 
A running version can be found at preprod.marketdata.trakx.io

## Avoid committing you secrets and keys 

In order to be able to run some integration tests, you should create a .env file with the following items : 
-   CRYPTOCOMPARE_API_KEY
-   INFURA_API_KEY
-   ETHERWALLET

And put the path of the file in src/Trakx.Tests/Tools/Secrets.cs

## Trakx.Tests.Tools namespace
This is home to parts of code that have been used to run as one-offs, in order to build indices compositions, publish indices on chain, test rebalancings, etc. A project is currently ongoing to give access to these parts of the code to non technical users through a website.

## Localisation
Please make sure that you are running the solution with EN-GB localisation.

## Running locally
Get a local SQL running
```bash
docker pull mcr.microsoft.com/mssql/server:2019-GA-ubuntu-16.04
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=changeme" -p 1433:1433 --name index-repository -d mcr.microsoft.com/mssql/server:latest
```
Get a local redis instance running
```bash 
docker pull redis
docker run --name maketdata-price-cache -d redis -p 6379:6379
```

## Running docker-compose
From the root folder run 
```bash 
docker-compose --file src/docker-compose.local.yml up --build &
```
