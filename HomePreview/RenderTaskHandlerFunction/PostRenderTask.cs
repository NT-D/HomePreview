
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft;
using HomePreviewCommon.Data;

namespace RenderTaskHandlerFunction
{
    public static class PostRenderTask
    {
        private static DocumentClient Client;
        private static readonly string EndpointUrl = Environment.GetEnvironmentVariable("CosmosDBServiceUrl");
        private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("CosmosDBKey");
        private static readonly string DatabaseName = "RenderParamDB";
        private static readonly string CollectionName = "RenderParamCollection";
        private static readonly string StorageUrl = "https://samplestorege.com";

        [FunctionName(nameof(PostRenderTask))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req,
            //IBinder inputCosmosBinder,
            [Queue(queueName: "render-task-queue", Connection = "AzureWebJobsStorage")] ICollector<string> QueueMessage,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            //Read body data in HTTP POST
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            //1. Check wheter we got parameter. If it is null, return 400 bad request
            if (data == null)
            {
                return new BadRequestObjectResult("please input room parameter");
            }
            else
            {
                //Make CosmosDB client
                if (Client == null)
                {
                    Client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
                    await Client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
                    await Client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseName), new DocumentCollection { Id = CollectionName });
                }

                //2. Check wheter same parameter in CosmosDB.
                var param = new RenderParam() { Roomsize = 10, Windowsize = 2 };//TODO: update with request body


                //2-1. If exists we return url to the client.

                //2-2. If it doesn't exists, functions create new document in Cosmos DB and get uid, then send parameter and uid to Queue.
                var response = await Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), param);

                //Add url created by ID and update document
                param.Id = response.Resource.Id;
                param.ImageUrl = $"{StorageUrl}/{param.Id}.png";
                await Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), param);

                QueueMessage.Add(JsonConvert.SerializeObject(param));

                //TODO: update 3. Return tetative url to the client
                return new OkObjectResult(param);
            }
        }
    }
}
