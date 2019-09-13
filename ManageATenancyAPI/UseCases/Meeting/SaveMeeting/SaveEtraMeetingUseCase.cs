using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Transform;
using ManageATenancyAPI.Gateways.CloseMeeting;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingAttendance;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingIssue;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting;
using ManageATenancyAPI.Services.Email;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Microsoft.EntityFrameworkCore.Internal;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting
{
    public class SaveEtraMeetingUseCase : ISaveEtraMeetingUseCase
    {
        private readonly ISaveEtraMeetingGateway _saveEtraMeetingGateway;
        private readonly ISaveEtraMeetingIssueGateway _saveEtraMeetingIssueGateway;
        private readonly ISaveEtraMeetingAttendanceGateway _saveEtraMeetingAttendanceGateway;
        private readonly ISaveEtraMeetingSignOffMeetingGateway _saveEtraMeetingSignOffMeetingGateway;
        private readonly ISendTraConfirmationEmailGateway _sendTraConfirmationEmailGateway;
        private readonly ICloseETRAMeetingGateway _closeETRAMeetingGateway;
        public SaveEtraMeetingUseCase(ISaveEtraMeetingGateway saveEtraMeetingGateway, 
            ISaveEtraMeetingIssueGateway saveEtraMeetingIssueGateway, 
            ISaveEtraMeetingAttendanceGateway saveEtraMeetingAttendanceGateway,
            ISaveEtraMeetingSignOffMeetingGateway saveEtraMeetingSignOffMeetingGateway,
            ISendTraConfirmationEmailGateway sendTraConfirmationEmailGateway,
            ICloseETRAMeetingGateway closeETRAMeetingGateway)
        {
            _saveEtraMeetingGateway = saveEtraMeetingGateway;
            _saveEtraMeetingIssueGateway = saveEtraMeetingIssueGateway;
            _saveEtraMeetingAttendanceGateway = saveEtraMeetingAttendanceGateway;
            _saveEtraMeetingSignOffMeetingGateway = saveEtraMeetingSignOffMeetingGateway;
            _sendTraConfirmationEmailGateway = sendTraConfirmationEmailGateway;
            _closeETRAMeetingGateway = closeETRAMeetingGateway;
        }

        public async Task<SaveEtraMeetingOutputModel> ExecuteAsync(SaveETRAMeetingInputModel request, IManageATenancyClaims claims, CancellationToken cancellationToken)
        {
            var etraMeeting = new ETRAMeeting
            {
                MeetingName = request.MeetingName,
                TraId = request.TRAId,
            };

            var outputModel = new SaveEtraMeetingOutputModel();
            outputModel.Name = request.MeetingName;

            var response = await _saveEtraMeetingGateway.CreateEtraMeeting(etraMeeting, claims, cancellationToken).ConfigureAwait(false);

            outputModel.Id = response != null ? response.InteractionId:new Guid() ;
            etraMeeting.Id = response!=null? response.InteractionId:new Guid();


            if (request.Issues != null && request.Issues.Any())
            {
                outputModel.Issues = new List<MeetingIssueOutputModel>();
                foreach (var requestIssue in request.Issues)
                {
                    var issue = await _saveEtraMeetingIssueGateway.CreateEtraMeetingIssue(etraMeeting, requestIssue, claims, cancellationToken).ConfigureAwait(false);
                    outputModel.Issues.Add(issue);
                }
            }

            var successfullySavedAttendees = await _saveEtraMeetingAttendanceGateway.CreateEtraAttendance(etraMeeting, request.MeetingAttendance, cancellationToken).ConfigureAwait(false);
            if(successfullySavedAttendees)
                outputModel.MeetingAttendance = request.MeetingAttendance;

            if (request.SignOff != null)
            {
                var signOffMeetingOutputModel = await _saveEtraMeetingSignOffMeetingGateway.SignOffMeetingAsync(response.InteractionId, request.SignOff, cancellationToken).ConfigureAwait(false);
                outputModel.SignOff = signOffMeetingOutputModel?.SignOff;

                outputModel.IsSignedOff = signOffMeetingOutputModel?.IsSignedOff ?? false;
                //if the meeting is signed off then close the meeting 
                //update interaction

                var closeMeetingInputModel = new CloseMeetingInputModel
                {
                    InteractionId = response.InteractionId,
                    IncidentId = response.IncidentId,
                    UpdatedByOfficer = claims.OfficerId
                };
                var IsMeetingClosed = _closeETRAMeetingGateway.CloseMeetingInteraction(closeMeetingInputModel, cancellationToken);
                //update the incident 
                var IsIncidentClosed = _closeETRAMeetingGateway.CloseMeetingIncident(closeMeetingInputModel, cancellationToken);
                //donot create annotation

                //if the meeting is signed off then send the email with the confirmation template 
            }

            var inputModel = new SendTraConfirmationEmailInputModel
            {
                MeetingId = response.InteractionId,
                OfficerName = claims?.FullName,
                TraId = request.TRAId,
                 IsMeetingSignedOff=outputModel.IsSignedOff
            };

            //put this email in the else condition of Is Signed off
            var sendTraConfirmationEmailOutputModel =  await _sendTraConfirmationEmailGateway.SendTraConfirmationEmailAsync(inputModel, cancellationToken).ConfigureAwait(false);

            outputModel.IsEmailSent = sendTraConfirmationEmailOutputModel?.IsSent ?? false;

            return outputModel;
        }
    }
}