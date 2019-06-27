using System.Threading;
using System.Threading.Tasks;

namespace ManageATenancyAPI.UseCases
{
    public interface IUseCaseWithResponse<TResponse>
    {
        Task<TResponse> ExecuteAsync(CancellationToken cancellationToken);
    }
}