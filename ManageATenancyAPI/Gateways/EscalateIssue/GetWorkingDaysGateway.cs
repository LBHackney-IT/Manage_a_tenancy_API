using System;

namespace ManageATenancyAPI.Gateways.EscalateIssue
{
    public class GetWorkingDaysGateway : IGetWorkingDaysGateway
    {
        public DateTime GetPreviousWorkingDaysFromToday(int number)
        {
            return DateTime.Today;
        }
    }
}