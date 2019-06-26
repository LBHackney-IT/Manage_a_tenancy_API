using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.SaveEtraMeeting;
using ManageATenancyAPI.Gateways.SaveEtraMeetingIssue;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Microsoft.EntityFrameworkCore.Internal;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting
{
    public class SaveEtraMeetingUseCase : ISaveEtraMeetingUseCase
    {
        private readonly ISaveEtraMeetingGateway _saveEtraMeetingGateway;
        private readonly ISaveEtraMeetingIssueGateway _saveEtraMeetingIssueGateway;

        public SaveEtraMeetingUseCase(ISaveEtraMeetingGateway saveEtraMeetingGateway, ISaveEtraMeetingIssueGateway saveEtraMeetingIssueGateway)
        {
            _saveEtraMeetingGateway = saveEtraMeetingGateway;
            _saveEtraMeetingIssueGateway = saveEtraMeetingIssueGateway;
        }

        public async Task<SaveETRAMeetingOutputModel> ExecuteAsync(SaveETRAMeetingInputModel request, IManageATenancyClaims claims, CancellationToken cancellationToken)
        {
            var etraMeeting = new ETRAMeeting
            {
                MeetingName = request.MeetingName,
                TraId = request.TRAId,
            };

            var outputModel = new SaveETRAMeetingOutputModel();

            var meetingId = await _saveEtraMeetingGateway.CreateEtraMeeting(etraMeeting, claims, cancellationToken).ConfigureAwait(false);

            outputModel.MeetingId = meetingId;
            etraMeeting.Id = meetingId;


            if (request.Issues != null && request.Issues.Any())
            {
                outputModel.Issues = new List<MeetingIssueOutputModel>();
                foreach (var requestIssue in request.Issues)
                {
                    var issue = await _saveEtraMeetingIssueGateway.CreateEtraMeetingIssue(etraMeeting, requestIssue, claims, cancellationToken).ConfigureAwait(false);
                    outputModel.Issues.Add(issue);
                }
            }

            return outputModel;
        }
    }
}