using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces.Housing;

namespace ManageATenancyAPI.Gateways.SaveEtraMeeting
{
    public interface ISaveEtraMeetingGateway
    {
        Task<Guid> CreateEtraMeeting(ETRAMeeting meeting, CancellationToken cancellationToken);
    }

    public class SaveEtraMeetingGateway: ISaveEtraMeetingGateway
    {
        private readonly IETRAMeetingsAction _etraMeetingsAction;

        public SaveEtraMeetingGateway(IETRAMeetingsAction etraMeetingsAction)
        {
            _etraMeetingsAction = etraMeetingsAction;
        }


        public Task<Guid> CreateEtraMeeting(ETRAMeeting meeting, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class ETRAMeeting
    {
        public string MeetingName { get; set; }
        public int TraId { get; set; }
    }
    
}
