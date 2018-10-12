using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models;

namespace ManageATenancyAPI.Interfaces
{
    public interface IUhwRepository
    {
        Task AddOrderDocumentAsync(string documentType, string workOrderReference, int workOrderId, string processComment);
    }
}
