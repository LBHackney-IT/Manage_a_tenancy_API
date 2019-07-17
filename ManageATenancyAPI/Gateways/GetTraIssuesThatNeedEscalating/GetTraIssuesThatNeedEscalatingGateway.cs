using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;

namespace ManageATenancyAPI.Gateways.GetTraIssuesThatNeedEscalating
{
    public class GetTraIssuesThatNeedEscalatingGateway : IGetTraIssuesThatNeedEscalatingGateway
    {
        private readonly IETRAMeetingsAction _etraMeetingsAction;

        public GetTraIssuesThatNeedEscalatingGateway(IETRAMeetingsAction etraMeetingsAction)
        {
            _etraMeetingsAction = etraMeetingsAction;
        }

        public Task<IList<TRAIssue>> GetTraIssuesThatNeedEscalating(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}