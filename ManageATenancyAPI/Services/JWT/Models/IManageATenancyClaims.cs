using System;

namespace ManageATenancyAPI.Services.JWT.Models
{
    public interface IManageATenancyClaims
    {

        Guid EstateOfficerLoginId { get; set; }
        Guid OfficerId { get; set; }
        string Username { get; set; }
        string FullName { get; set; }
        Guid AreaManagerId { get; set; }
        Guid OfficerPatchId { get; set; }
        int AreaId { get; set; }
    }
}