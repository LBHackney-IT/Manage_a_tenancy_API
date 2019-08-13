using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.EscalateIssue;
using ManageATenancyAPI.Gateways.GetTraIssuesThatNeedEscalating;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting;
using ManageATenancyAPI.Gateways.SendEscalationEmailGateway;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Newtonsoft.Json;

namespace ManageATenancyAPI.UseCases.Meeting.EscalateIssues
{
    public class EscalateIssuesUseCase : IEscalateIssuesUseCase
    {
        private readonly IGetTraIssuesThatNeedEscalatingGateway _getTraIssuesThatNeedEscalatingGateway;
        private readonly IEscalateIssueGateway _escalateIssueGateway;
        private readonly ISendEscalationEmailGateway _sendEscalationEmailGateway;

        private readonly IJsonPersistanceService _jsonPersistenceService;
        public EscalateIssuesUseCase(IGetTraIssuesThatNeedEscalatingGateway getTraIssuesThatNeedEscalatingGateway, 
            IEscalateIssueGateway escalateIssueGateway, IGetWorkingDaysGateway getWorkingDaysGateway, 
            ISendEscalationEmailGateway sendEscalationEmailGateway, 
            IJsonPersistanceService jsonPersistanceServices)
        {
            _getTraIssuesThatNeedEscalatingGateway = getTraIssuesThatNeedEscalatingGateway;
            _escalateIssueGateway = escalateIssueGateway;
            _sendEscalationEmailGateway = sendEscalationEmailGateway;
            _jsonPersistenceService = jsonPersistanceServices;
        }

        public async Task<EscalateIssuesOutputModel> ExecuteAsync(CancellationToken cancellationToken)
        {
            var issues = await _getTraIssuesThatNeedEscalatingGateway.GetTraIssuesThatNeedEscalating(cancellationToken).ConfigureAwait(false);

            if(issues == null || !issues.Any())
                return new EscalateIssuesOutputModel();

            var outputModel = new EscalateIssuesOutputModel();

            await EscalateIssues(cancellationToken, issues, outputModel);

            await SendEcalationEmail(cancellationToken, outputModel, issues);

            return outputModel;
        }

        private async Task SendEcalationEmail(CancellationToken cancellationToken, EscalateIssuesOutputModel outputModel,
            IList<EscalateMeetingIssueInputModel> issues)
        {
            var serviceAreaEmails = await _jsonPersistenceService.DeserializeStream<List<TRAIssueServiceArea>>("service-area-emails").ConfigureAwait(false);
            var areaManagerDetails = await _jsonPersistenceService.DeserializeStream<List<AreaManagerDetails>>("area-manager-emails").ConfigureAwait(false);

            for (int i = 0; i < outputModel?.SuccessfullyEscalatedIssues.Count; i++)
            {
                var issue = issues.ElementAtOrDefault(i);
                var sendEmailResponse = await _sendEscalationEmailGateway.SendEscalationEmailAsync(new SendEscalationEmailInputModel
                {
                    Issue = issue,
                    ServiceArea = serviceAreaEmails.Where(w => w.IssueId.ToString().Equals(issue.IssueType.IssueId)).FirstOrDefault(),
                    AreaManagerDetails = areaManagerDetails.Where(w=> w.AreaId.ToString().Equals(issue.AreaId)).FirstOrDefault()
                }, cancellationToken).ConfigureAwait(false);

                outputModel.SuccessfullyEscalatedIssues[i].ServiceOfficerEmailSent = sendEmailResponse.SentToServiceAreaOfficer;
                outputModel.SuccessfullyEscalatedIssues[i].ServiceAreaManagerEmailSent = sendEmailResponse.SentToServiceAreaManager;
                outputModel.SuccessfullyEscalatedIssues[i].AreaHousingManagerEmailSent = sendEmailResponse.AreaHousingManager;
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