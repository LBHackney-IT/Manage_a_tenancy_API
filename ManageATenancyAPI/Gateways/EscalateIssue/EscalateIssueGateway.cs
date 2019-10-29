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
        public async Task<EscalateIssueOutputModel> EscalateIssueAsync(EscalateIssueInputModel inputModel, CancellationToken cancellationToken)
        {
            var isSuccessful = await _etraMeetingsAction.EscalateIssue(inputModel?.Issue, cancellationToken).ConfigureAwait(false);
            return new EscalateIssueOutputModel
            {
                Successful = isSuccessful
            };
        }
    }
}