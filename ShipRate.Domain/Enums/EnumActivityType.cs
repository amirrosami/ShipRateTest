namespace ShipRate.Domain.Enums
{
    public enum EnumActivityType
    {
        // Communication
        IncomingCall = 1,
        OutgoingCall = 2,
        EmailSent = 3,
        EmailReceived = 4,
        WhatsApp = 5,
        SMS = 6,

        // Sales
        Meeting = 20,
        OnlineMeeting = 21,
        OnsiteVisit = 22,
        Negotiation = 23,

        // Commercial
        QuoteRequested = 40,
        QuoteSent = 41,
        QuoteRevised = 42,
        QuoteApproved = 43,
        QuoteRejected = 44,

        // Logistics Consulting
        RouteConsultation = 60,
        CustomsConsultation = 61,
        TransitTimeDiscussion = 62,
        SampleShipment = 63,

        // System
        AutoEmailSent = 90,
        AutoReminder = 91,
        LeadAssigned = 92,
        StatusChanged = 93
    }
}
