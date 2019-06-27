using System;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting
{
    public class JpegPersistenceService : BasePersistenceService, IJpegPersistenceService
    {
        public JpegPersistenceService(IOptions<JpegPersistenceServiceConfiguration> config, IOptions<S3Configuration> s3) : base(config, s3)
        {
            if (config.Value.FileType != FileType.Jpeg) throw new ArgumentException();
        }
    }
}