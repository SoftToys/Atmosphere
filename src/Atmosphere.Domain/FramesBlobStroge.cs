using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Atmosphere.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Atmosphere.Contracts.Models;

namespace Atmosphere.Domain
{
    public class FramesBlobStorage : IFrameRepository
    {
        private IEnvironmentConfig _config;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;

        public FramesBlobStorage(IEnvironmentConfig config)
        {
            _config = config;

            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_config.StorageConnectionString);

            // Create the blob client.
            _blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            _container = _blobClient.GetContainerReference($"atmo-{DateTime.Now.ToString("MM-dd")}");

            // Create the container if it doesn't already exist.

            var exists = _container.CreateIfNotExistsAsync().Result;
            if (!exists)
            {
                _container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
        }

        public async Task<DataResponse<Uri>> Store(Stream imgStream)
        {
            var blockBlob = _container.GetBlockBlobReference(Guid.NewGuid().ToString());
            // Create or overwrite the "myblob" blob with contents from a local file.
            await blockBlob.UploadFromStreamAsync(imgStream);
            return new DataResponse<Uri>() { Data = blockBlob.Uri };
        }

        public async Task<DataResponse<List<Uri>>> List(DateTime day)
        {
            // Loop over items within the container and output the length and URI.            

            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;
            DataResponse<List<Uri>> resp = new DataResponse<List<Uri>>();
            resp.Data = new List<Uri>();

            try
            {
                do
                {
                    // The prefix is required when listing blobs from the service client. The prefix must include
                    // the container name.
                    resultSegment = await _blobClient.ListBlobsSegmentedAsync(day.ToString("HH-dd"), continuationToken);
                    foreach (var blob in resultSegment.Results)
                    {
                        resp.Data.Add(blob.Uri);
                    }

                    // Get the continuation token.
                    continuationToken = resultSegment.ContinuationToken;

                } while (continuationToken != null);

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
            return resp;
        }

    }
}
