namespace ShipRate.Common.ExternalServices.Accouting
{
    public interface IAccountingRepository
    {
        Task<AccDocument> GetAccDocument(Guid accId);
        Task UpdateAccDoumentStatus(string newStatus);
        Task AddNewAccDocument(AccDocument accDocument);
        Task DeleteAccDocument(string accId);
        Task UpdateAccDocument(AccDocument accDocument);
    }
}


/*
get document from db -> convert document datamodel to a Request body of external Accounting Service 
--> send it
*/










