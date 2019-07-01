using System.Threading;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public interface IEmailService
    {
        Task<SendTraConfirmationEmailOutputModel> SendTraConfirmationEmailAsync(SendTraConfirmationEmailInputModel inputModel, CancellationToken cancellationToken);
    }
}