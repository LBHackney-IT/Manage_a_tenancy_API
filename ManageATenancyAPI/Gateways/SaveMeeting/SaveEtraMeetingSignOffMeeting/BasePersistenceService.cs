using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting
{
    public abstract class BasePersistenceService : IPersistenceService
    {
        private readonly BasePersistenceServiceConfiguration _configuration;
        private readonly IOptions<S3Configuration> _s3Configuration;
        private readonly IAmazonS3 _s3;

        public BasePersistenceService(IOptions<BasePersistenceServiceConfiguration> configuration, IOptions<S3Configuration> s3Configuration)
        {
            _configuration = configuration?.Value;
            if (_configuration == null)
                throw new ArgumentNullException(nameof(BasePersistenceServiceConfiguration), "Configuration is not present");
            _s3Configuration = s3Configuration;

            _s3 = new AmazonS3Client(new BasicAWSCredentials(_s3Configuration.Value?.AccessKey, _s3Configuration.Value?.Secret), new AmazonS3Config { RegionEndpoint = RegionEndpoint.EUWest2 });
        }

        public async Task<byte[]> GetAsync(string id)
        {
            var transferUtility = new TransferUtility(_s3);

            var fileName = GetFileName(id);

            using (var fileStream = transferUtility.OpenStream(_s3Configuration.Value?.BucketName, fileName))
            {
                var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public async Task SaveAsync(string id, byte[] payload)
        {

            try
            {
                var transferUtility = new TransferUtility(_s3);

                using (var ms = new MemoryStream(payload))
                {
                    await transferUtility.UploadAsync(ms, _s3Configuration.Value?.BucketName, GetFileName(id));
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }

        private string GetFileName(string id)
        {
            var fileName = $"{_configuration.ProjectName}/{_configuration.FileType}/{id}.{_configuration.Extension}";
            return fileName;
        }
    }
}