﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingAttendance;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingIssue;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.Tests.Unit.Services;
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
        private readonly IEmailService _emailService;

        public SaveEtraMeetingUseCase(ISaveEtraMeetingGateway saveEtraMeetingGateway, 
            ISaveEtraMeetingIssueGateway saveEtraMeetingIssueGateway, 
            ISaveEtraMeetingAttendanceGateway saveEtraMeetingAttendanceGateway,
            ISaveEtraMeetingSignOffMeetingGateway saveEtraMeetingSignOffMeetingGateway,
            IEmailService emailService)
        {
            _saveEtraMeetingGateway = saveEtraMeetingGateway;
            _saveEtraMeetingIssueGateway = saveEtraMeetingIssueGateway;
            _saveEtraMeetingAttendanceGateway = saveEtraMeetingAttendanceGateway;
            _saveEtraMeetingSignOffMeetingGateway = saveEtraMeetingSignOffMeetingGateway;
            _emailService = emailService;
        }

        public async Task<SaveEtraMeetingOutputModel> ExecuteAsync(SaveETRAMeetingInputModel request, IManageATenancyClaims claims, CancellationToken cancellationToken)
        {
            var etraMeeting = new ETRAMeeting
            {
                MeetingName = request.MeetingName,
                TraId = request.TRAId,
            };

            var outputModel = new SaveEtraMeetingOutputModel();

            var meetingId = await _saveEtraMeetingGateway.CreateEtraMeeting(etraMeeting, claims, cancellationToken).ConfigureAwait(false);

            outputModel.Id = meetingId;
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

            await _saveEtraMeetingAttendanceGateway.CreateEtraAttendance(etraMeeting, request.MeetingAttendance, cancellationToken).ConfigureAwait(false);

            if (request.SignOff != null)
            {
                var signOffMeetingOutputModel = await _saveEtraMeetingSignOffMeetingGateway.SignOffMeetingAsync(meetingId, request.SignOff, cancellationToken).ConfigureAwait(false);
                outputModel.IsSignedOff = signOffMeetingOutputModel?.IsSignedOff ?? false;
            }

            

            return outputModel;
        }
    }
}