namespace ShipRate.WebApp.ViewModels.CustomersVms
{
    public class LostReasonCategoryViewModel
    {
        public int Id { get; set; }
        public string LostReasonCategoryName { get; set; } = string.Empty;
        public string LostReasonCategoryLocalName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class LostReasonCategoryListViewModel
    {
        public List<LostReasonCategoryListItem> Categories { get; set; } = new();
        public int? SelectedCategoryId { get; set; }
    }

    public class LostReasonCategoryListItem
    {
        public int Id { get; set; }
        public string LostReasonCategoryName { get; set; } = string.Empty;
        public string LostReasonCategoryLocalName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int ReasonsCount { get; set; }
    }

    // ViewModels/LostReasonViewModel.cs

    public class LostReasonViewModel
    {
        public int Id { get; set; }
        public int LostReasonCategoryId { get; set; }
        public string LostReasonName { get; set; } = string.Empty;
        public string LostReasonLocalName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class LostReasonListViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<LostReasonListItem> Reasons { get; set; } = new();
    }

    public class LostReasonListItem
    {
        public int Id { get; set; }
        public string LostReasonName { get; set; } = string.Empty;
        public string LostReasonLocalName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int LostReasonCategoryId { get; set; }
    }



}
