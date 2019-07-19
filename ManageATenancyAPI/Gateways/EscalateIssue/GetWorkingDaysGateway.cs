using System;

namespace ManageATenancyAPI.Gateways.EscalateIssue
{
    public class GetWorkingDaysGateway : IGetWorkingDaysGateway
    {
        public DateTime GetPreviousWorkingDaysFromToday(int numberOfDays)
        {
            throw new NotImplementedException();
        }

        public DateTime GetPreviousDaysFromToday(int numberOfDays)
        {
            return DateTime.Today.AddDays(-numberOfDays);
        }
    }
}