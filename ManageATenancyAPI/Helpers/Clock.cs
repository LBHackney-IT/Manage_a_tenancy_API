using System;
using ManageATenancyAPI.Interfaces;

namespace ManageATenancyAPI.Helpers
{
    public class Clock : IClock
    {
        public DateTime Now => DateTime.Now;
    }
}