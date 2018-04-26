
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace RenderTaskHandlerVM
{
    class QueueCheck
    {
        private static readonly string _queueName = "render-task-queue";
        private readonly CloudStorageAccount _account;

        public QueueCheck(CloudStorageAccount account)
        {
            _account = account;          
        }

        public async Task<bool> RunAsync()
        {
            var client = _account.CreateCloudQueueClient();
            var queue = client.GetQueueReference(_queueName);

            // If there is no queue named _queueName, create the queue
            await queue.CreateIfNotExistsAsync();

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

                            await queue.DeleteMessageAsync(message);
                        }
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }
        }

    }
}
