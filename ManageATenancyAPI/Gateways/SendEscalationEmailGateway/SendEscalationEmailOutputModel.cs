namespace ManageATenancyAPI.Gateways.SendEscalationEmailGateway
{
    public class SendEscalationEmailOutputModel
    {
        public bool Successful { get; set; }
        public bool SentToServiceAreaOfficer { get; set; }
        public bool SentToServiceAreaManager { get; set; }
        public bool AreaHousingManager { get; set; }
    }
}