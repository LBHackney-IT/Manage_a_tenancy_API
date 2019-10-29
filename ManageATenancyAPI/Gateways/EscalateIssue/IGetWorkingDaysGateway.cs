using System;

namespace ManageATenancyAPI.Gateways.EscalateIssue
{
    public interface IGetWorkingDaysGateway
    {
        DateTime GetPreviousWorkingDaysFromToday(int numberOfDays);
        DateTime GetPreviousDaysFromToday(int numberOfDays);
    }
}