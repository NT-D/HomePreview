# HomePreview

## Data scheme

Request
```javascript
{
    "roomsize":12,
    "windowsize":5
}
```

Response
```javascript
{
    "id": "5866bda9-2006-45c5-812e-ec3b692a5848",
    "roomsize": 12,
    "windowsize": 5,
    "imageUrl": "https://<Your storage url>/public/5866bda9-2006-45c5-812e-ec3b692a5848.png"
}
```

## How to run

You need to make `local.settings.json` like this for running Azure Functions.

```javascript
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "CosmosDBConnectionString": "<Your Cosmos DB Connection String>",
    "CosmosDBServiceUrl": "https://<your url>.documents.azure.com:443/",
    "CosmosDBKey": "<Your Cosmos DB Key>",
    "StorageUrl": "https://<your storage url>.blob.core.windows.net",
    "StorageContainerName": "public"
  }
}
```