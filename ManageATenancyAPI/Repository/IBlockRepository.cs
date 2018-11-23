﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;

namespace ManageATenancyAPI.Repository
{
    public interface IBlockRepository
    {
        Task<IEnumerable<Block>> GetBlocksByEstateId(string estateId);
    }
}
