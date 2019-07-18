﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.EscalateIssue;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.Gateways.GetTraIssuesThatNeedEscalating
{
    public class GetTraIssuesThatNeedEscalatingGateway : IGetTraIssuesThatNeedEscalatingGateway
    {
        private readonly IETRAMeetingsAction _etraMeetingsAction;
        private readonly IGetWorkingDaysGateway _getWorkingDaysGateway;

        public GetTraIssuesThatNeedEscalatingGateway(IETRAMeetingsAction etraMeetingsAction, IGetWorkingDaysGateway getWorkingDaysGateway)
        {
            _etraMeetingsAction = etraMeetingsAction;
            _getWorkingDaysGateway = getWorkingDaysGateway;
        }

        public async Task<IList<MeetingIssueOutputModel>> GetTraIssuesThatNeedEscalating(CancellationToken cancellationToken)
        {
            var fifteenWorkingDaysAgo = _getWorkingDaysGateway.GetPreviousWorkingDaysFromToday(0);
            var outputModel = await _etraMeetingsAction.GetAllEtraIssuesThatNeedEscalatingAsync(fifteenWorkingDaysAgo, cancellationToken).ConfigureAwait(false);
            return outputModel?.IssuesThatNeedEscalating;
        }
    }
}