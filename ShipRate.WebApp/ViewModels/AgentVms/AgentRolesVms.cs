using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShipRate.WebApp.ViewModels.AgentVms
{
    // ViewModels/AgentRoles/AgentRolesListViewModel.cs
    public class AgentRolesListViewModel
    {
        public Guid AgentId { get; set; }
        public string AgentName { get; set; }
        public List<AgentRoleRowVm> Items { get; set; } = new();
    }
    public class AgentTerritoriesListViewModel
    {
        public Guid AgentId { get; set; }
        public string AgentName { get; set; }
        public List<AgentTerritoryRowVm> Items { get; set; } = new();
    }

    public class AgentTerritoryRowVm
    {
        public Guid Id { get; set; }
        public string TerritoryName { get; set; }
        public bool IsActive { get; set; }
    }




    // ViewModels/AgentRoles/AgentRoleRowVm.cs
    public class AgentRoleRowVm
    {
        public Guid Id { get; set; }
        public string CarrierName { get; set; }
        public string AgentRoleTypeName { get; set; }
        public decimal? CommissionRatePercent { get; set; }
        public bool IsActive { get; set; }
    }

    // ViewModels/AgentRoles/AgentRoleFormViewModel.cs
    public class AgentRoleFormViewModel
    {
        public Guid Id { get; set; }
        public Guid AgentId { get; set; }
        public Guid? CarrierId { get; set; }
        public int? AgentRoleTypeId { get; set; }
        public decimal? CommissionRatePercent { get; set; }
        public bool IsActive { get; set; }

        // Dropdowns
        public SelectList Carriers { get; set; }
        public SelectList RoleTypes { get; set; }
    }


    public class AgentTerritoryFormViewModel
    {
        public Guid Id { get; set; }
        public Guid AgentId { get; set; }
        public Guid TerritoryId { get; set; }
        public bool IsActive { get; set; }
        // Dropdowns
        public SelectList Territories { get; set; }
    }
}
