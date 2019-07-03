using System.Threading;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Services.Email
{
    public interface ISendTraConfirmationEmailGateway
    {
        Task<SendTraConfirmationEmailOutputModel> SendTraConfirmationEmailAsync(SendTraConfirmationEmailInputModel inputModel, CancellationToken cancellationToken);
    }
}