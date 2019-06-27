using System.Threading;
using System.Threading.Tasks;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting
{
    public interface IGetEtraMeetingIssueTypesUseCase:IUseCaseWithResponse<GetEtraMeetingIssueTypesResponse>
    {

    }

    public class GetEtraMeetingIssueTypesResponse
    {

    }

    public class GetEtraMeetingIssueTypesUseCase : IGetEtraMeetingIssueTypesUseCase
    {
        public Task<GetEtraMeetingIssueTypesResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
     
}