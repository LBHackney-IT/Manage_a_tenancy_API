using System;

namespace ManageATenancyAPI.Gateways.EscalateIssue
{
    public interface IGetWorkingDaysGateway
    {
        DateTime GetPreviousWorkingDaysFromToday(int number);
    }
}