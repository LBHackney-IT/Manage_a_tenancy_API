using System.Collections.Generic;

namespace ManageATenancyAPI.UseCases.Meeting.EscalateIssues
{
    public class EscalateIssuesOutputModel
    {
        public IList<TRAIssue> IssuesToEscalate { get; set; }
        public IList<TRAIssue> SuccessfullyEscalatedIssues { get; set; }
        public IList<TRAIssue> FailedToEscalateIssues { get; set; }
    }
}