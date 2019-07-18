using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.EscalateIssue;
using ManageATenancyAPI.Gateways.GetTraIssuesThatNeedEscalating;
using ManageATenancyAPI.Gateways.SendEscalationEmailGateway;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.EscalateIssues
{
    public class EscalateIssuesUseCase : IEscalateIssuesUseCase
    {
        private readonly IGetTraIssuesThatNeedEscalatingGateway _getTraIssuesThatNeedEscalatingGateway;
        private readonly IEscalateIssueGateway _escalateIssueGateway;
        private readonly ISendEscalationEmailGateway _sendEscalationEmailGateway;

        public EscalateIssuesUseCase(IGetTraIssuesThatNeedEscalatingGateway getTraIssuesThatNeedEscalatingGateway, IEscalateIssueGateway escalateIssueGateway, IGetWorkingDaysGateway getWorkingDaysGateway, ISendEscalationEmailGateway sendEscalationEmailGateway)
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

            var outputModel = new EscalateIssuesOutputModel();

            await EscalateIssues(cancellationToken, issues, outputModel);


            for (int i = 0; i < outputModel?.SuccessfullyEscalatedIssues.Count; i++)
            {
                var issue = issues.ElementAtOrDefault(i);
                await _sendEscalationEmailGateway.SendEscalationEmailAsync(new SendEscalationEmailInputModel
                {
                    Issue = issue,
                }, cancellationToken).ConfigureAwait(false);
            }
            

            return null;
        }

        private async Task EscalateIssues(CancellationToken cancellationToken, IList<MeetingIssueOutputModel> issues,
            EscalateIssuesOutputModel outputModel)
        {
            for (int i = 0; i < issues.Count; i++)
            {
                var issue = issues.ElementAtOrDefault(i);
                var escalateIssueOutputModel = await _escalateIssueGateway.EscalateIssueAsync(
                    new EscalateIssueInputModel
                    {
                        Issue = issue
                    }, cancellationToken).ConfigureAwait(false);

                if (escalateIssueOutputModel.Successful)
                {
                    if(outputModel.SuccessfullyEscalatedIssues == null)
                        outputModel.SuccessfullyEscalatedIssues = new List<MeetingIssueOutputModel>();
                    outputModel.SuccessfullyEscalatedIssues.Add(issue);
                }
                else
                {
                    if (outputModel.FailedToEscalateIssues == null)
                        outputModel.FailedToEscalateIssues = new List<MeetingIssueOutputModel>();
                    outputModel.FailedToEscalateIssues.Add(issue);
                }
                    
            }
        }
    }
}