| Windows | MacOs | Ubuntu64
| :---- | :------ | :---- |
| [![Build Status]()]() | [![Build Status]()]() | [![Build Status]()]()

# marketdata-feeds-mvp

A web service providing historical and realtime market data for our trackers, the data is calculated based on data sourced on CoinMarketCap.com and CryptoCompare.com

## Tracker list

This tool serves market data for the following trackers

- Levered BTC x2
- Levered BTC x3
- Inverse BTC
- Inverse BTC Levered x2
- Inverse BTC Levered x3
- Levered ETH x2
- Levered ETH x3
- Inverse ETH
- Inverse ETH Levered x2
- Inverse ETH Levered x3
- Long Top 5 Market cap
- Long Levered Top 5 Market cap x2
- Inverse Top 5 Market cap
- Inverse Levered Top 5 Market x2
- Long Top 20 Market cap
- Long Levered Top 20 Market cap x2
- Inverse Top 20 Market Cap
- Inverse Levered Top 20 Market x2

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