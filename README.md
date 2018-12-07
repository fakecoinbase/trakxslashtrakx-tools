| Windows | MacOs | Ubuntu64
| :---- | :------ | :---- |
| [![Build Status]()]() | [![Build Status]()]() | [![Build Status]()]()

# marketdata-feeds-mvp

A web service providing historical and realtime market data for our trackers, the data is calculated based on data sourced on CoinMarketCap.com and CryptoCompare.com

## Tracker list

This tool serves market data for the following trackers

- L2BTC - Long BTC Levered x2
- L3BTC - Long BTC Levered x3
- I1BTC - Inverse BTC
- I2BTC - Inverse BTC Levered x2
- I3BTC - Inverse BTC Levered x3
- L2ETH - Long ETH Levered x2
- L3ETH - Long ETH Levered x3
- I1ETH - Inverse ETH
- I2ETH - Inverse ETH Levered x2
- I3ETH - Inverse ETH Levered x3
- L1MC005 - Long Top 5 Market Cap
- L2MC005 - Long Top 5 Market Cap Levered x2
- I1MC005 - Inverse Top 5 Market Cap
- I2MC005 - Inverse Top 5 Market Cap Levered x2
- L1MC005 - Long Top 20 Market Cap
- L1MC020 - Long Top 20 Market Cap Levered x2
- I1MC020 - Inverse Top 20 Market Cap
- I1MC020 - Inverse Top 20 Market Cap Levered x2

## Available endpoints

### Rest API :

- price
- all/coinlist
- top/totalvol
- top/volumes
- pricehistorical
- pricemultifull
- top/pair
- all/exchanges

### Websocket :

Subscription : type~market~symbol~currency