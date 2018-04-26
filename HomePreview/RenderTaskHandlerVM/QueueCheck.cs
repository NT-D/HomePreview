
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using HomePreviewCommon.Data;

namespace RenderTaskHandlerVM
{
    class QueueCheck
    {
        private static readonly string _queueName = "render-task-queue";
        private static readonly string _blobName = "public";

        private readonly CloudStorageAccount _account;

        public QueueCheck(CloudStorageAccount account)
        {
            _account = account;          
        }

        public async Task<bool> RunAsync()
        {
            var queueClient = _account.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(_queueName);

            var blobClient = _account.CreateCloudBlobClient();
            var blob = blobClient.GetContainerReference(_blobName);

            // If there is no queue named _queueName, create the queue
            await queue.CreateIfNotExistsAsync();

            // If there is no blob named _blobName, create the blob
            await blob.CreateIfNotExistsAsync();

            while (true)
            {
                await queue.FetchAttributesAsync();
                int count = queue.ApproximateMessageCount ?? 0;

                if (count > 0)
                {
                    // GetMessagesAsync support maximum count of 32
                    if (count > 32)
                        count = 32;

                    foreach (var message in await queue.GetMessagesAsync(count))
                    {
                        if (message != null)
                        {
                            //read json in the queue
                            var paramData = JsonConvert.DeserializeObject<RenderParam>(message.AsString);

                            using (var file = new FileStream("param.json", FileMode.Create))
                            using (var writer = new StreamWriter(file, Encoding.UTF8))
                            {
                                writer.Write(message.AsString);
                            }

                            // start Blender
                            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = "blender -b -P render.py", };
                            Process proc = new Process() { StartInfo = startInfo, };
                            proc.Start();
                            proc.WaitForExit();

                            // upload rendered image named Result.png
                            CloudBlockBlob cloudBlockBlob = blob.GetBlockBlobReference($"{paramData.Id}.png");
                            await cloudBlockBlob.UploadFromFileAsync("Result.png");
                            await queue.DeleteMessageAsync(message);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }
        }

    }
}
