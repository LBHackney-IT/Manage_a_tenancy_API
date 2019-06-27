using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting
{
    public interface ISaveEtraMeetingSignOffMeetingGateway
    {
        Task<bool> SignOffMeetingAsync(Guid meetingId, SignOff signOff, CancellationToken cancellationToken);
    }

    public class SaveEtraMeetingSignOffMeetingGateway: ISaveEtraMeetingSignOffMeetingGateway
    {
        private readonly IETRAMeetingsAction _etraMeetingsAction;
        private readonly IJpegPersistenceService _jpegPersistenceService;

        public SaveEtraMeetingSignOffMeetingGateway(IETRAMeetingsAction etraMeetingsAction, IJpegPersistenceService jpegPersistenceService)
        {
            _etraMeetingsAction = etraMeetingsAction;
            _jpegPersistenceService = jpegPersistenceService;
        }

        public async Task<bool> SignOffMeetingAsync(Guid meetingId, SignOff signOff, CancellationToken cancellationToken)
        {
            var finaliseMeetingRequest = new FinaliseETRAMeetingRequest
            {
                Name = signOff.Name,
                Role = signOff.Role,
            };

            if (!string.IsNullOrEmpty(signOff.Signature))
            {
                var signatureGuid = Guid.NewGuid();
                var bytes = System.Convert.FromBase64String(signOff.Signature);
                await _jpegPersistenceService.SaveAsync(signatureGuid.ToString(), bytes).ConfigureAwait(false);
                finaliseMeetingRequest.SignatureId = signatureGuid;
            }

            var response = await _etraMeetingsAction.FinaliseMeeting(meetingId.ToString(), finaliseMeetingRequest).ConfigureAwait(false);
            return response.IsFinalised;
        }
    }

    public interface IPersistenceService
    {
        Task SaveAsync(string id, byte[] payload);
        Task<byte[]> GetAsync(string id);
    }

    public enum FileType
    {
        None,
        Pdf,
        Email,
        Jpeg
    }



    public class S3Configuration
    {
        public string RegionEndpoint { get; set; }
        public string AccessKey { get; set; }
        public string Secret { get; set; }
        public string BucketName { get; set; }
    }

    public class BasePersistenceServiceConfiguration
    {
        public FileType FileType { get; set; }
        public string Extension { get; set; }
        public string ProjectName { get; set; }
    }

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

    public interface IJpegPersistenceService : IPersistenceService
    {

    }

    public class JpegPersistenceService : BasePersistenceService, IJpegPersistenceService
    {
        public JpegPersistenceService(IOptions<JpegPersistenceServiceConfiguration> config, IOptions<S3Configuration> s3) : base(config, s3)
        {
            if (config.Value.FileType != FileType.Jpeg) throw new ArgumentException();
        }
    }

    public class JpegPersistenceServiceConfiguration : BasePersistenceServiceConfiguration
    {

    }

}