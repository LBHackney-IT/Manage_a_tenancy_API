using System.Threading;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public interface ISendTraConfirmationEmailGateway
    {
        Task<SendTraConfirmationEmailOutputModel> SendTraConfirmationEmailAsync(SendTraConfirmationEmailInputModel inputModel, CancellationToken cancellationToken);
    }
}