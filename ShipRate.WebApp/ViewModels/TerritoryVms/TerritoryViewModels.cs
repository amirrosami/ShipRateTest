using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.ViewModels.TerritoryVms
{
    public class TerritorySearchViewModel
    {
        public string? TerritoryName { get; set; }
        public string? CityName { get; set; }
        public int TerritoryPage { get; set; } = 1;
        public int TerritoryPageSize { get; set; } = 10;
        public Guid? SelectedTerritoryId { get; set; }
        public int CityPage { get; set; } = 1;
        public int CityPageSize { get; set; } = 10;
    }


    public class TerritoryListViewModel
    {
        public Guid Id { get; set; }
        public string TerritoryName { get; set; } = "";
        public string Remarks { get; set; } = "";
        public int CityCount { get; set; }
    }
    public class TerritoryEditViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Territory Name is required.")]
        [MaxLength(100)]
        public string TerritoryName { get; set; } = "";

        [MaxLength(200)]
        public string? Remarks { get; set; }

    }

   public class TerritoryCityCreateEditDto
   {
        public Guid Id { get; set; }
        public Guid TerritoryId { get; set; }
        public Guid CityId { get; set; }
        public string? CityName { get; set; }
    }

    public class TerritoryCityRowViewModel
    {
        public Guid Id { get; set; }          // TerritoryCities.Id
        public Guid CityId { get; set; }
        public string LocationName { get; set; } = "";
        public bool IsActive { get; set; }
    }

    public class LocationSearchResult
    {
        public Guid Id { get; set; }
        public string LocationName { get; set; } = "";
    }

}
