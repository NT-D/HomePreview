
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;

namespace RenderTaskHandlerFunction
{
    public static class PostRenderTask
    {
        [FunctionName(nameof(PostRenderTask))]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req,
            [Queue(queueName: "render-task-queue", Connection = "AzureWebJobsStorage")] ICollector<string> QueueMessage,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            //1. Check wheter we got parameter. If it is null, return 400 bad request
            if (data == null)
            {
                return new BadRequestObjectResult("please input room parameter");
            }
            else
            {

                //2. Check wheter same parameter in CosmosDB.

                //2-1. If exists we return url to the client.

                //2-2. If it doesn't exists, functions create new document in Cosmos DB and get uid, then send parameter and uid to Queue.
                QueueMessage.Add(JsonConvert.SerializeObject(data));

                //TODO: update 3. Return tetative url to the client
                return (ActionResult)new OkObjectResult(data.roomsize);
            }
        }
    }
}
