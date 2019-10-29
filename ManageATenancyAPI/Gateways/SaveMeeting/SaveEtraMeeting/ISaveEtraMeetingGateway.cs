using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Services.JWT.Models;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting
{
    public interface ISaveEtraMeetingGateway
    {
        Task<ETRAMeetingOutPutModel> CreateEtraMeeting(ETRAMeeting meeting, IManageATenancyClaims manageATenancyClaims, CancellationToken cancellationToken);
    }
}