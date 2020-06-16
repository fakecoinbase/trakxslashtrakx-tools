# Debugging this project

## IndiceRepository database available locally
Make sure you have an instance of this database available locally. If not, you can start the Trakx.MarketData.Server locally and it will create one for you.

## Redis Cache
Make sure that you have a Redis cache available locally. If not, you can start one using this command:
`docker run --name maketdata-price-cache -d redis --port 6379:6379`
and ensure it is available by trying
`redis-cli`.
