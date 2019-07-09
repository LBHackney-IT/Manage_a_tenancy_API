using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Services.JWT.Models;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting
{
    public class SaveEtraMeetingGateway: ISaveEtraMeetingGateway
    {
        private readonly IETRAMeetingsAction _etraMeetingsAction;

        public SaveEtraMeetingGateway(IETRAMeetingsAction etraMeetingsAction)
        {
            _etraMeetingsAction = etraMeetingsAction;
        }

        public async Task<Guid> CreateEtraMeeting(ETRAMeeting meeting, IManageATenancyClaims manageATenancyClaims, CancellationToken cancellationToken)
        {
            var etraIssue = new ETRAIssue
            {
                estateOfficerId = manageATenancyClaims.OfficerId.ToString(),
                subject = "c1f72d01-28dc-e711-8115-70106faa6a11",
                estateOfficerName = manageATenancyClaims.FullName,

                officerPatchId = manageATenancyClaims.OfficerPatchId.ToString(),


                // area Id 
                areaName = manageATenancyClaims.AreaId.ToString(),
                //setting manager Id assigns it to a manager
                //managerId = manageATenancyClaims.AreaManagerId.ToString(),
                ServiceRequest = new CRMServiceRequest
                {
                    Description = meeting.MeetingName,
                    //Generic Subject Id from Dynamics 365 list of subjects..............
                    //In the Subjects Custom Entities... table
                    //Which relates to Tenancy Management interactions... I don't know..
                    Subject = "c1f72d01-28dc-e711-8115-70106faa6a11",
                    //  CreatedBy = manageATenancyClaims.OfficerId.ToString()
                },

                TRAId = meeting.TraId.ToString(),
                ///ETRA - Values are in Dynamics 365 - some lookup table?
                natureOfEnquiry = "28",

                //meeting enquiry subject in the CRM option set hackney_enquirysubject the value for the meeting is 100000219
                enquirySubject = "100000219",
                // todo: comment what this process type is??
                processType = "1"
            };
            var response = await _etraMeetingsAction.CreateETRAMeeting(etraIssue).ConfigureAwait(false);
            return response.InteractionId.GetValueOrDefault();
        }
    }

    public class ETRAMeeting
    {
        public Guid Id { get; set; }
        public string MeetingName { get; set; }
        public int TraId { get; set; }
    }
}
