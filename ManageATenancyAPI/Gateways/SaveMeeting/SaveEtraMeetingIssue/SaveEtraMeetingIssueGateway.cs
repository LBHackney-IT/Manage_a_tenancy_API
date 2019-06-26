using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using ETRAMeeting = ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting.ETRAMeeting;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingIssue
{
    public class SaveEtraMeetingIssueGateway : ISaveEtraMeetingIssueGateway
    {
        private readonly IETRAMeetingsAction _etraMeetingsAction;

        public SaveEtraMeetingIssueGateway(IETRAMeetingsAction etraMeetingsAction)
        {
            _etraMeetingsAction = etraMeetingsAction;
        }

        public async Task<MeetingIssueOutputModel> CreateEtraMeetingIssue(ETRAMeeting meeting, MeetingIssue meetingIssue, IManageATenancyClaims manageATenancyClaims, CancellationToken cancellationToken)
        {
            var etraIssue = new ETRAIssue
            {
                estateOfficerId = manageATenancyClaims.EstateOfficerLoginId.ToString(),
                subject = "c1f72d01-28dc-e711-8115-70106faa6a11",
                estateOfficerName = manageATenancyClaims.FullName,

                officerPatchId = manageATenancyClaims.OfficerPatchId.ToString(),

                areaName = manageATenancyClaims.AreaId.ToString(),

                managerId = manageATenancyClaims.AreaManagerId.ToString(),

                ServiceRequest = new CRMServiceRequest
                {
                    Description = meetingIssue.IssueNote,
                    //Generic Subject Id from Dynamics 365 list of subjects..............
                    //In the Subjects Custom Entities... table
                    //Which relates to Tenancy Management interactions... I don't know..
                    Subject = "c1f72d01-28dc-e711-8115-70106faa6a11",
                    CreatedBy = manageATenancyClaims.EstateOfficerLoginId.ToString()
                },

                TRAId = meeting.TraId.ToString(),

                parentInteractionId = meeting.Id.ToString(),
                ///ETRA - Values are in Dynamics 365 - some lookup table?
                natureOfEnquiry = "28",
                // todo: comment what this process type is??
                //ETRA I think - Check Dynamics 365 reference tables... which one.. not sure
                processType = "3",

                enquirySubject = meetingIssue.IssueTypeId,
                issueLocation = meetingIssue.IssueLocationName
            };

            var response = await _etraMeetingsAction.CreateETRAMeeting(etraIssue).ConfigureAwait(false);

            return new MeetingIssueOutputModel
            {
                Id = response.InteractionId.GetValueOrDefault(),
                IssueNote = meetingIssue.IssueNote,
                IssueTypeId = meetingIssue.IssueTypeId,
                IssueLocationName = meetingIssue.IssueLocationName,
            };
        }
    }
}