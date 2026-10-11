namespace ShipRate.WebApp.ViewModels.CustomersVms
{
    // ViewModels/Crm/CustomerGroupingBaseVM.cs
    public class CustomerGroupingBaseVM
    {
        public Guid Id { get; set; }
        public string CustomerGroupingBaseName { get; set; }
        public bool IsActive { get; set; }
        public int GroupCount { get; set; }
    }

    // ViewModels/Crm/CustomerGroupVM.cs
    public class CustomerGroupVM
    {
        public Guid Id { get; set; }
        public Guid CustomerGroupingBaseId { get; set; }
        public string CustomerGroupName { get; set; }
        public bool IsActive { get; set; }
        public int MemberCount { get; set; }
    }

    // ViewModels/Crm/CustomerBelongsToGroupVM.cs
    public class CustomerBelongsToGroupVM
    {
        public Guid Id { get; set; }
        public Guid CustomerGroupId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime EnterDate { get; set; }
        public DateTime? ExitDate { get; set; }
        public string EnterRemarks { get; set; }
        public string ExitRemarks { get; set; }
        public bool IsActive { get; set; }
    }

    // ViewModels/Crm/CustomerBelongsToGroupFormVM.cs
    public class CustomerBelongsToGroupFormVM
    {
        public Guid? Id { get; set; }
        public Guid CustomerGroupId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime EnterDate { get; set; } = DateTime.Now;
        public DateTime? ExitDate { get; set; }
        public string EnterRemarks { get; set; }
        public string ExitRemarks { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ViewModels/Crm/CustomerGroupingBaseFormVM.cs
    public class CustomerGroupingBaseFormVM
    {
        public Guid? Id { get; set; }
        public string CustomerGroupingBaseName { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ViewModels/Crm/CustomerGroupFormVM.cs
    public class CustomerGroupFormVM
    {
        public Guid? Id { get; set; }
        public Guid CustomerGroupingBaseId { get; set; }
        public string CustomerGroupName { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ViewModels/Crm/CustomerSearchResultVM.cs
    public class CustomerSearchResultVM
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
    }
}
