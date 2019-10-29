using System;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Services.JWT.Models;

namespace ManageATenancyAPI.UseCases.Meeting.GetMeeting
{
    public interface IGetEtraMeetingUseCase
    {
        Task<GetEtraMeetingOutputModel> ExecuteAsync(GetEtraMeetingInputModel request, CancellationToken cancellationToken);
    }
}