using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using Newtonsoft.Json;

namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public class GetAreaManagerInformationGateway : IGetAreaManagerInformationGateway
    {
        public async Task<IList<AreaManagerDetails>> GetAreaManagerDetails(CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var path = Path.Combine(directory, "area-manager-emails.json");

            var json = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);

            var list = JsonConvert.DeserializeObject<List<AreaManagerDetails>>(json);
            return list;
        }
    }
}