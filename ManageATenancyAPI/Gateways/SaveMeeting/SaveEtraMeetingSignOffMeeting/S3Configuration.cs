namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting
{
    public class S3Configuration
    {
        public string RegionEndpoint { get; set; }
        public string AccessKey { get; set; }
        public string Secret { get; set; }
        public string BucketName { get; set; }
    }
}