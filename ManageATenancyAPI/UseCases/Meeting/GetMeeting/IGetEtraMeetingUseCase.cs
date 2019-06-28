using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.Boundary;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.GetMeeting
{
    public interface IGetEtraMeetingUseCase
    {
        Task<GetEtraMeetingOutputModel> ExecuteAsync(GetEtraMeetingInputModel request, IManageATenancyClaims claims, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Output model for getting a meeting
    /// </summary>
    public class GetEtraMeetingOutputModel : IMeetingOutputModel
    {
        /// <summary>
        /// Refers to the TenancyInteractionId in Dynamics 365
        /// </summary>
        public Guid MeetingId { get; set; }

        public IList<MeetingIssueOutputModel> Issues { get; set; }
        public SignOff SignOff { get; set; }
        public bool IsSignedOff { get; set; }
    }

    /// <summary>
    /// InputModel for getting a meeting
    /// </summary>
    public class GetEtraMeetingInputModel
    {
        public Guid MeetingId { get; set; }
    }
}