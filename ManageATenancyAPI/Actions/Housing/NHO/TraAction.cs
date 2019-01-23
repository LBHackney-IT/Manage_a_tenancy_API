using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Models;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class TraAction : ITraAction
    {
        private readonly ITRARepository _traRepository;
        public TraAction(ITRARepository traRepository)
        {
            _traRepository = traRepository;
        }

        public async Task<bool> Exists(string traName)
        {
            return await _traRepository.Exists(traName);
        }
        public async Task<TRA> Find(string traName)
        {
            return await _traRepository.Find(traName);
        }

        public async Task<TRA> Create(string name, string notes, string email, int areaId, Guid patchId)
        {
            return await _traRepository.Create(name, notes, email, areaId, patchId);
        }

        public void UpdateNotes(int traId, string notes)
        {
            _traRepository.UpdateNotes(traId, notes);
        }

        public void UpdateEmail(int traId, string email)
        {
            _traRepository.UpdateEmail(traId, email);
        }

    }
}
