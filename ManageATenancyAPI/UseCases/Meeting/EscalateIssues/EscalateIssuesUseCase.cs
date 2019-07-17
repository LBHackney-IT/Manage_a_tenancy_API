using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.EscalateIssue;
using ManageATenancyAPI.Gateways.GetTraIssuesThatNeedEscalating;
using ManageATenancyAPI.Gateways.SendEscalationEmailGateway;

namespace ManageATenancyAPI.UseCases.Meeting.EscalateIssues
{
    public class EscalateIssuesUseCase : IEscalateIssuesUseCase
    {
        private readonly IGetTraIssuesThatNeedEscalatingGateway _getTraIssuesThatNeedEscalatingGateway;
        private readonly IEscalateIssueGateway _escalateIssueGateway;
        private readonly ISendEscalationEmailGateway _sendEscalationEmailGateway;

        public EscalateIssuesUseCase(IGetTraIssuesThatNeedEscalatingGateway getTraIssuesThatNeedEscalatingGateway, IEscalateIssueGateway escalateIssueGateway, ISendEscalationEmailGateway sendEscalationEmailGateway)
        {
            _getTraIssuesThatNeedEscalatingGateway = getTraIssuesThatNeedEscalatingGateway;
            _escalateIssueGateway = escalateIssueGateway;
            _sendEscalationEmailGateway = sendEscalationEmailGateway;
        }

        public async Task<EscalateIssuesOutputModel> ExecuteAsync(CancellationToken cancellationToken)
        {
            var issues = await _getTraIssuesThatNeedEscalatingGateway.GetTraIssuesThatNeedEscalating(cancellationToken).ConfigureAwait(false);

            if(issues == null || !issues.Any())
                return new EscalateIssuesOutputModel();
            return null;
        }
    }
}