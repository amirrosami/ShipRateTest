namespace ShipRate.Common.ExternalServices.Tax
{
    public enum TaxInvoiceStatus
    {
        NotSent = 0,
        SentSuccessfully = 1,
        FailedToSend = 2,
        FailedAfterSend = 3,
        InProgress = 4,
    }
}

