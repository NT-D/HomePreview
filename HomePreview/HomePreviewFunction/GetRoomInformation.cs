using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using HomePreviewFunction.Models;
using System.Collections.Generic;
using System.Net;

namespace HomePreviewFunction
{
    public static class GetRoomInformation
    {
        [FunctionName(nameof(GetRoomInformation))]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req,
            IBinder inputCosmosBinder,
            [DocumentDB(databaseName: "homeimagedb", collectionName: "jpcollection", Id = "id", ConnectionStringSetting = "CosmosDBConnection")]IAsyncCollector<HomeInformation> outputDocuments,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // Get request body
            var input = await req.Content.ReadAsAsync<HomeInformation>();

            //Make CosmosDB attribute for fetching data
            var inputAttribute = new DocumentDBAttribute(databaseName: "homeimagedb", collectionName: "jpcollection");
            inputAttribute.ConnectionStringSetting = "CosmosDBConnection";
            inputAttribute.SqlQuery = $"SELECT c.url FROM c WHERE c.floor = {input.floor}";//TODO: Need to update query
            var documents = await inputCosmosBinder.BindAsync<IEnumerable<HomeInformation>>(inputAttribute);

            if (documents.Count() != 0)
            {
                //We retrun only 1st document temporally.
                var existingDocument = documents.FirstOrDefault();
                log.Info("Found existing image and returned it's information");
                return req.CreateResponse(HttpStatusCode.OK, new ResponseHomeInfo() { url = existingDocument.url });
            }
            else
            {
                //We need to create Unique image name.
                input.url = "https://hpstorage.blob.core.windows.net/image/Dirrogate_Airport_Stereoscopic_360_VR.jpg";
                await outputDocuments.AddAsync(input);
                log.Info("Created new document and returned it's information. Client need to do polling for fetching the image");
                return req.CreateResponse(HttpStatusCode.Created, new ResponseHomeInfo() { url = input.url });
            }
        }
    }
}
