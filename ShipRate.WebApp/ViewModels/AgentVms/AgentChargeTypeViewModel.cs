namespace ShipRate.WebApp.ViewModels.AgentVms
{
    public class AgentChargeTypeViewModel
    {
        public Guid AgentId { get; set; }
        public string AgentName { get; set; }
        public Dictionary<string, List<ChargeTypeItem>> ChargeTypesByCategory { get; set; }
    }

    public class ChargeTypeItem
    {
        public int ChargeTypeId { get; set; }
        public string ChargeTypeName { get; set; }
        public bool IsSelected { get; set; }
    }

    public class SaveAgentChargeTypesRequest
    {
        public Guid AgentId { get; set; }
        public List<int> SelectedChargeTypeIds { get; set; }
    }
}
