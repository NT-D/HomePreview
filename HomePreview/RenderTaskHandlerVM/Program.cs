using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace RenderTaskHandlerVM
{
    class Program
    {
        private static readonly string StorageUrl = "StorageUrl";
        private static readonly string StorageKey = "StorageKey";
        static void Main(string[] args)
        {
            // create storage account
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(StorageUrl,StorageKey), true);

            // start checking queue
            new QueueCheck(storageAccount).RunAsync().Wait();
        }
    }
}
