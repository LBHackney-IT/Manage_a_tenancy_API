﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting;
using ManageATenancyAPI.Services.Email;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.SignOffMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.SignOffMeeting
{
    public class SignOffMeetingUseCase: ISignOffMeetingUseCase
    {
        private readonly ISaveEtraMeetingSignOffMeetingGateway _saveEtraMeetingSignOffMeetingGateway;
        private readonly ISendTraConfirmationEmailGateway _sendTraConfirmationEmailGateway;

        public SignOffMeetingUseCase(ISaveEtraMeetingSignOffMeetingGateway saveEtraMeetingSignOffMeetingGateway, ISendTraConfirmationEmailGateway sendTraConfirmationEmailGateway)
        {
            _saveEtraMeetingSignOffMeetingGateway = saveEtraMeetingSignOffMeetingGateway;
            _sendTraConfirmationEmailGateway = sendTraConfirmationEmailGateway;
        }

        public async Task<SignOffMeetingOutputModel> ExecuteAsync(SignOffMeetingInputModel request, IMeetingClaims claims, CancellationToken cancellationToken)
        {

            var outputModel = await _saveEtraMeetingSignOffMeetingGateway.SignOffMeetingAsync(claims.MeetingId, request?.SignOff, cancellationToken).ConfigureAwait(false);

            var inputModel = new SendTraConfirmationEmailInputModel
            {
                MeetingId = claims.MeetingId,
                OfficerName = claims.OfficerName,
                TraId = claims.TraId
            };

            var sendTraConfirmationEmailOutputModel = await _sendTraConfirmationEmailGateway.SendTraConfirmationEmailAsync(inputModel, cancellationToken).ConfigureAwait(false);

            outputModel.IsEmailSent = sendTraConfirmationEmailOutputModel?.IsSent ?? false;

            return outputModel;
        }
    }
}