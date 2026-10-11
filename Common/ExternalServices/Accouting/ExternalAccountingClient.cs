namespace ShipRate.Common.ExternalServices.Accouting
{
    public abstract class ExternalAccountingClient
    {
        protected ExternalAccountingClient() { }
        public abstract void Login();
        public abstract Task<ExternalAccoutingClientResponse<string>> SendAccDocumentAsync(AccDocument accDocument);
    }
}



