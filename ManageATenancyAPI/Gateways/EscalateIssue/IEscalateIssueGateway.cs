using System.Threading;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Gateways.EscalateIssue
{
    public interface IEscalateIssueGateway
    {
        Task<EscalateIssueOutputModel> EscalateIssueAsync(EscalateIssueInputModel inputModel, CancellationToken cancellationToken);
    }
}