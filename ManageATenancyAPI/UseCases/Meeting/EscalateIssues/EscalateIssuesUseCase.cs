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
        private readonly IGetServiceAreaInformationGateway _getServiceAreaInformationGateway;
        private readonly IGetAreaManagerInformationGateway _getAreaManagerInformationGateway;

        public EscalateIssuesUseCase(IGetTraIssuesThatNeedEscalatingGateway getTraIssuesThatNeedEscalatingGateway, 
            IEscalateIssueGateway escalateIssueGateway, IGetWorkingDaysGateway getWorkingDaysGateway, 
            ISendEscalationEmailGateway sendEscalationEmailGateway, 
            IGetServiceAreaInformationGateway getServiceAreaInformationGateway, 
            IGetAreaManagerInformationGateway getAreaManagerInformationGateway)
        {
            _getTraIssuesThatNeedEscalatingGateway = getTraIssuesThatNeedEscalatingGateway;
            _escalateIssueGateway = escalateIssueGateway;
            _sendEscalationEmailGateway = sendEscalationEmailGateway;
            _getServiceAreaInformationGateway = getServiceAreaInformationGateway;
            _getAreaManagerInformationGateway = getAreaManagerInformationGateway;
        }

        public async Task<EscalateIssuesOutputModel> ExecuteAsync(CancellationToken cancellationToken)
        {
            var issues = await _getTraIssuesThatNeedEscalatingGateway.GetTraIssuesThatNeedEscalating(cancellationToken).ConfigureAwait(false);

            if(issues == null || !issues.Any())
                return new EscalateIssuesOutputModel();

            var outputModel = new EscalateIssuesOutputModel();

            await EscalateIssues(cancellationToken, issues, outputModel);

            await SendEcalationEmail(cancellationToken, outputModel, issues);

            return null;
        }

        private async Task SendEcalationEmail(CancellationToken cancellationToken, EscalateIssuesOutputModel outputModel,
            IList<EscalateMeetingIssueInputModel> issues)
        {
            var serviceAreaEmails = await _getServiceAreaInformationGateway.GetServiceAreaInformation(cancellationToken).ConfigureAwait(false);
            var areaManagerDetails = await _getAreaManagerInformationGateway.GetAreaManagerDetails(cancellationToken).ConfigureAwait(false);

            for (int i = 0; i < outputModel?.SuccessfullyEscalatedIssues.Count; i++)
            {
                var issue = issues.ElementAtOrDefault(i);
                await _sendEscalationEmailGateway.SendEscalationEmailAsync(new SendEscalationEmailInputModel
                {
                    Issue = issue,
                    ServiceArea = serviceAreaEmails.Where(w => w.IssueId.ToString().Equals(issue.IssueType.IssueId)).FirstOrDefault(),
                    AreaManagerDetails = areaManagerDetails.Where(w=> w.AreaId.ToString().Equals(issue.AreaId)).FirstOrDefault()
                }, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task EscalateIssues(CancellationToken cancellationToken, IList<EscalateMeetingIssueInputModel> issues,
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
                        outputModel.SuccessfullyEscalatedIssues = new List<EscalateMeetingIssueInputModel>();
                    outputModel.SuccessfullyEscalatedIssues.Add(issue);
                }
                else
                {
                    if (outputModel.FailedToEscalateIssues == null)
                        outputModel.FailedToEscalateIssues = new List<EscalateMeetingIssueInputModel>();
                    outputModel.FailedToEscalateIssues.Add(issue);
                }
            }
        }
    }
}