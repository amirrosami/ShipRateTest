namespace ShipRate.Application.ViewModels.Dashboard
{
    public sealed class MonthlyRevenueByCurrencyVm
    {
        public int CurrencyId { get; init; }
        public string CurrencyCode { get; init; } = string.Empty;

        public decimal CurrentMonthAmount { get; init; }
        public decimal PreviousMonthAmount { get; init; }

        public decimal ChangePercent
        {
            get
            {
                if (PreviousMonthAmount == 0)
                    return CurrentMonthAmount > 0 ? 100m : 0m;

                return ((CurrentMonthAmount - PreviousMonthAmount) / PreviousMonthAmount) * 100m;
            }
        }

        public bool IsIncrease => ChangePercent >= 0;
    }

    //public sealed class DashboardVm
    //{
    //    public List<MonthlyRevenueByCurrencyVm> MonthlyRevenueByCurrencies { get; init; } = [];
    //}


    public sealed class MonthlyFinanceByCurrencyVm
    {
        public int CurrencyId { get; init; }
        public string CurrencyCode { get; init; } = string.Empty;

        public decimal RevenueAmount { get; init; }
        public decimal ExpenseAmount { get; init; }

        public decimal NetAmount => RevenueAmount - ExpenseAmount;

        public bool IsProfitable => NetAmount >= 0;
    }

    public sealed class DashboardVm
    {
        public List<MonthlyFinanceByCurrencyVm> MonthlyFinanceByCurrencies { get; init; } = [];
        public List<TransportationModeChartItemVm> TransportationModes { get; set; } = [];
        public ShipmentVolumeChartVm ShipmentVolume { get; set; } = new();
        public List<TopShippingRouteVm> TopShippingRoutes { get; set; } = [];
        public CustomerGrowthChartVm CustomerGrowth { get; set; } = new();




    }

    public sealed class CustomerGrowthChartVm
    {
        public List<string> Labels { get; init; } = [];
        public List<int> Counts { get; init; } = [];
        public int TotalCustomers { get; init; }
        public int CurrentMonthCount { get; init; }
        public int PreviousMonthCount { get; init; }

        public decimal ChangePercent
        {
            get
            {
                if (PreviousMonthCount == 0)
                    return CurrentMonthCount > 0 ? 100m : 0m;

                return ((decimal)(CurrentMonthCount - PreviousMonthCount)
                        / PreviousMonthCount) * 100m;
            }
        }

        public bool IsIncrease => ChangePercent >= 0;
    }




    public sealed class TransportationModeChartItemVm
    {
        public int TransportationModeId { get; init; }

        public string Title { get; init; } = string.Empty;

        public int Count { get; init; }

        public decimal Percentage { get; init; }

        public string Color { get; init; } = "#696cff";
    }

    public sealed class ShipmentVolumeChartVm
    {
        public string Range { get; set; } = "weekly";

        public List<string> Labels { get; set; } = [];

        public List<int> CurrentPeriodData { get; set; } = [];

        public List<int> PreviousPeriodData { get; set; } = [];

        public string CurrentPeriodLabel { get; set; } = string.Empty;

        public string PreviousPeriodLabel { get; set; } = string.Empty;
    }

    public sealed class TopShippingRouteVm
    {
        public string Route { get; init; } = string.Empty;

        public int ShipmentCount { get; init; }
    }




}
