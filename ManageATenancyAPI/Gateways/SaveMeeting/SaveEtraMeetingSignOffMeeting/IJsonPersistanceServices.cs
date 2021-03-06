﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting
{
    public interface IJsonPersistanceService : IPersistenceService
    {
        Task<T> DeserializeStream<T>(string filename);
    }
}
