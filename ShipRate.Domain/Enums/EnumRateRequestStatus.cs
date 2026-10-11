namespace ShipRate.Domain.Enums
{
    public enum EnumRateRequestStatus
    {
        InitByCustomer = 1,
        SentByCustomer = 2,
        SentToSalesManager = 3,
        SentToSalesman = 4,
        SentToAgents = 5,
        RateToCustomer = 6,
        SalesManagerRejected = 7,
        SalesmanRejected = 8,
        CustomerRejected = 9,
        CustomerAccepted = 10,
        Booked = 11
    }
}
