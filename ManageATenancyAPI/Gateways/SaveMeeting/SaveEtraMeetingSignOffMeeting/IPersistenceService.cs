using System.Threading.Tasks;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting
{
    public interface IPersistenceService
    {
        Task SaveAsync(string id, byte[] payload);
        Task<byte[]> GetAsync(string id);
    }
}