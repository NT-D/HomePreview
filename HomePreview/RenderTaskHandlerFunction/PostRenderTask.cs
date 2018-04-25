
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
using System.Linq;

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
            [Queue(queueName: "render-task-queue", Connection = "AzureWebJobsStorage")] ICollector<string> QueueMessage,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var paramData = JsonConvert.DeserializeObject<RenderParam>(requestBody);

            //1. Check wheter we got parameter. If it is null, return 400 bad request
            if (paramData == null)
            {
                return new BadRequestObjectResult("please input room parameter");
            }
            else
            {
                //2. Make CosmosDB client
                if (Client == null)
                {
                    Client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
                    await Client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
                    await Client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseName), new DocumentCollection { Id = CollectionName });
                }

                //3. Check wheter same parameter exists in CosmosDB.
                //I want to use FirstorDefault(), but CosmosDb doesn't support it..
                List<RenderParam> existingParamList = Client.CreateDocumentQuery<RenderParam>(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), new FeedOptions() { MaxItemCount = 1 })
                    .Where(p => p.Roomsize == paramData.Roomsize && p.Windowsize == paramData.Windowsize)
                    .AsEnumerable()
                    .ToList();

                if (existingParamList.Count != 0)
                {
                    //3-1. If exists we return url to the client.
                    return new OkObjectResult(existingParamList[0]);
                }
                else
                {
                    //3-2. If it doesn't exists, functions create new document in Cosmos DB and create id for document, then send parameter and uid to Queue.
                    var response = await Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), paramData);

                    //Add image url which create with id and update document
                    paramData.Id = response.Resource.Id;
                    paramData.ImageUrl = $"{StorageUrl}/{paramData.Id}.png";
                    await Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), paramData);

                    QueueMessage.Add(JsonConvert.SerializeObject(paramData));

                    //TODO: update 3. Return tetative url to the client
                    return new OkObjectResult(paramData);
                }

            }
        }
    }
}
