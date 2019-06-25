using System;
using System.Collections.Generic;
using System.Text;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.Tests.v2.Helper
{
    public static class SaveMeetingInputModelHelper
    {
        public static SaveETRAMeetingInputModel Create()
        {
            return new SaveETRAMeetingInputModel
            {
                MeetingAttendance = new MeetingAttendees
                {
                    Attendees = 1
                },
                Issues = new List<MeetingIssue>
                {
                    new MeetingIssue
                    {
                        IssueTypeId = "100000501",
                        IssueLocationName = "De Beauvoir Estate  1-126 Fermain Court",
                        IssueNote = "Bad things have happened please fix"
                    },
                    new MeetingIssue
                    {
                        IssueTypeId = "100000501",
                        IssueLocationName = "De Beauvoir Estate  1-126 Fermain Court",
                        IssueNote = "Bad things have happened please fix 2"
                    }
                },
                SignOff = new SignOff
                {
                    Name = "Jeff Pinkham",
                    Role = "chair",
                    Signature = ""
                }
            };
        }
    }
}
