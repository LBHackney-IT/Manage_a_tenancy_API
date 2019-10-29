using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting
{
    public class JsonPersistanceService : BasePersistenceService, IJsonPersistanceService
    {

        public JsonPersistanceService(IOptions<JsonPersistanceServiceConfiguration> config, IOptions<S3Configuration> s3) : base(config, s3)
        {
            if (config.Value.FileType != FileType.Json) throw new ArgumentException();

        }

        public async Task<T> DeserializeStream<T>(string filename)
        {
           var bytes= base.GetAsync(filename);

            Stream stream = new MemoryStream(bytes.Result);

           using (StreamReader reader = new StreamReader(stream))

            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer ser = new JsonSerializer();
                return  ser.Deserialize<T>(jsonReader);
            }
        }
    }
}
