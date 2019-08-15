namespace ManageATenancyAPI.Services.Email
{
    public static class EmailKeys
    {
        public static string EmailAddress => "email address";
        public static string Subject => "subject";
        public static string MeetingUrl => "meetingUrl";
        public static string TraName => "traName";
        public static string OfficerName => "officerName";
        public static string OfficerAddress => "officerAddress";

        public static class EscalationEmail
        {
            public static string ServiceAreaManagerName => "service_area_manager_name";
            public static string ServiceAreaOfficerName => "service_area_officer_name";
            public static string DateResponseWasDue => "date_response_was_due";
            public static string IssueType => "issue_type";
            public static string ReferenceNumber => "reference_number";
            public static string Location => "location";

            public static string IssueNotes => "issue_notes";
            public static string TraName => "tra_name";
            public static string ResponseLine => "response_link";
            public static string HousingOfficerName => "housing_officer_name";
            public static string NHOAddress => "nho_address";
        }
    }
}