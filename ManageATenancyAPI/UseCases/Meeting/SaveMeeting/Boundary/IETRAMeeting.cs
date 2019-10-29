using System.ComponentModel.DataAnnotations;
using ManageATenancyAPI.Models;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    public interface IETRAMeeting
    {

        [Required]
        string estateOfficerId { get; set; }
        string subject { get; set; }
        [Required]
        string estateOfficerName { get; set; }
        [Required]
        string officerPatchId { get; set; }
        [Required]
        string areaName { get; set; }
        [Required]
        string managerId { get; set; }
        CRMServiceRequest ServiceRequest { get; set; }
        /// <summary>
        /// 0 - ManageTenancy Interaction - requests that aren't process
        /// 1 - ETRA Meeting
        /// 2 - Post Visit action
        /// 3 - ETRA Issue
        /// </summary>
        string processType { get; set; }
        [Required]
        string TRAId { get; set; }
    }
}