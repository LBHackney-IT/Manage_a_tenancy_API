using System;

namespace ManageATenancyAPI.Interfaces
{
    public interface IClock
    {
        DateTime Now { get; }
    }
}