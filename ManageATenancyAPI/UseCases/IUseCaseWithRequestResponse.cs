using System.Threading;
using System.Threading.Tasks;

namespace ManageATenancyAPI.UseCases
{
    public interface IUseCaseWithRequestResponse<TRequest, TResponse>
    {
        Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
    }
}