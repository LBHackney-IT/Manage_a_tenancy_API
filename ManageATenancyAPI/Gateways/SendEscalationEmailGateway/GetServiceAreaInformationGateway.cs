using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public class GetServiceAreaInformationGateway : IGetServiceAreaInformationGateway
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public GetServiceAreaInformationGateway(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IList<TRAIssueServiceArea>> GetServiceAreaEmails(CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var path = Path.Combine(directory, "service-area-emails.json");

            var json = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);

            var list = JsonConvert.DeserializeObject<List<TRAIssueServiceArea>>(json);
            return list;
        }
    }
}