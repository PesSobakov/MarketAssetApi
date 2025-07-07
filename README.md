# To run application using docker
Run in command line
```
docker-compose up -d --build
```
Before using application you need initialise and seed database

Visit site http://127.0.0.1:7017/api/MarketAsset/seed

Or use http://127.0.0.1:7017/swagger and run request /api/MarketAsset/seed

# Endpoints
Http Seed database
```
/api/MarketAsset/seed
```
Http Get list of supported market assets
```
/api/MarketAsset/market-assets
```
Http Get historical price information for specific asset(s)
```
/api/MarketAsset/historical-prices
```
WebSocket Get real time price information for specific asset(s)
```
/api/MarketAsset/real-time-prices
```

# Postman
To run http requests in postman you can use collection and environment
```
MarketAssetApi/MarketAssetApi/MarketAsset Api HTTP.postman_collection.json
MarketAssetApi/MarketAssetApi/MarketAsset Api.postman_environment.json
```

# WebSocket
You can run WebSocket request in [sandbox](https://app.gosandy.io/?url=https://raw.githubusercontent.com/PesSobakov/MarketAssetApi/refs/heads/master/MarketAssetApi/MarketAsset%20Api%20Websocket.json)
