namespace ShipRate.Common.ExternalServices.Accouting
{
    public class AccountingService
    {
        private readonly ExternalAccountingClient _extAccClient;
        private readonly IAccountingRepository _accountingRepository;
        public AccountingService(ExternalAccountingClient extAccClient, IAccountingRepository accountingRepository)
        {
            _extAccClient = extAccClient;
            _accountingRepository = accountingRepository;
        }

        public async Task AddNewAccDocument(AccDocumentDto accDocumentDto)
        {
            var id = Guid.CreateVersion7();
            await _accountingRepository.AddNewAccDocument(new AccDocument());
        }
        public async Task UpdateAccDocumentStatus(string newStatus)
        {
            await _accountingRepository.UpdateAccDoumentStatus(newStatus);
        }
        public async Task UpdateAccDocument(AccDocumentDto accDocumentDto)
        {
            await _accountingRepository.UpdateAccDocument(new AccDocument());
        }

        public async Task SendAccDocument(Guid accId)
        {
            var document = await _accountingRepository.GetAccDocument(accId);
            if (document == null)
            {
                throw new Exception("Document is Null");
            }

            var response = await _extAccClient.SendAccDocumentAsync(document);
            if (response.StatusCode == 200)
            {
                document.Status = "Sent";
                document.ReferenceId = response.Data;
                await _accountingRepository.UpdateAccDocument(document);
            }
            else
            {
                document.Status = "FailedToSend";
                document.Dscp = response.Message;
                await _accountingRepository.UpdateAccDocument(document);
                throw new Exception($"{response.ErrorCode}: {response.Message}");
            }

        }
    }
}


/*
get document from db -> convert document datamodel to a Request body of external Accounting Service 
--> send it
*/










