using System;
using ManageATenancyAPI.Interfaces;

namespace ManageATenancyAPI.Helpers
{
    public class Clock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}