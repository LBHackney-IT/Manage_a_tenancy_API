using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Interfaces
{
    public interface ILoggerAdapter<T>
    {
        // add just the logger methods your app uses
        void LogInformation(string message);

        void LogError(string message, params object[] args);
    }
}
