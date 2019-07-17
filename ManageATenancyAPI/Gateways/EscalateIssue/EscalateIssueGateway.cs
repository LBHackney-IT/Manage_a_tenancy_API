using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces.Housing;

namespace ManageATenancyAPI.Gateways.EscalateIssue
{
    public class EscalateIssueGateway : IEscalateIssueGateway
    {
        private readonly IETRAMeetingsAction _etraMeetingsAction;

        public EscalateIssueGateway(IETRAMeetingsAction etraMeetingsAction)
        {
            _etraMeetingsAction = etraMeetingsAction;
        }
        public Task<EscalateIssueOutputModel> EscalateIssueAsync(EscalateIssueInputModel inputModel, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}