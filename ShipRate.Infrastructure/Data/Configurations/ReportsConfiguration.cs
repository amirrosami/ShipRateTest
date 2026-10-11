using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.Reports;

namespace ShipRate.Infrastructure.Data.Configurations
{
    public class ReportCategoryConfiguration : IEntityTypeConfiguration<ReportCategory>
    {
        public void Configure(EntityTypeBuilder<ReportCategory> builder)
        {
            builder.ToTable("ReportCategory", "rpt");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.CategoryName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CategoryLocalName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(200);
            builder.Property(x => x.IconClass).HasMaxLength(50);
            builder.Property(x => x.ColorClass).HasMaxLength(50);


            //Seed Data
            builder.HasData(
                new ReportCategory { Id = 1, CategoryName = "Commercial Reports", CategoryLocalName = "گزارشات بازرگانی/بازاریابی", ColorClass = "bg-info", IconClass = "bi bi-water" },
                new ReportCategory { Id = 2, CategoryName = "Operational Reports", CategoryLocalName = "گزارشات عملیاتی", ColorClass = "bg-priamry", IconClass = "bi bi-airplane" },
                new ReportCategory { Id = 3, CategoryName = "Container Reports", CategoryLocalName = "گزارشات کانتینر", ColorClass = "bg-warning", IconClass = "bi bi-box" },
                new ReportCategory { Id = 4, CategoryName = "Finance Reports", CategoryLocalName = "گزارشات مالی", ColorClass = "bg-error", IconClass = "bi bi-currency-dollar" },
                new ReportCategory { Id = 5, CategoryName = "Management Reports", CategoryLocalName = "گزارشات مدیریتی", ColorClass = "bg-error", IconClass = "bi bi-clipboard-data" }
            );

        }
    }

    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports", "rpt");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.ReportKey).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ReportName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ReportLocalName).IsRequired().HasMaxLength(200);


            builder.Property(x => x.ControllerName).IsRequired().HasMaxLength(70);
            builder.Property(x => x.ActionName).IsRequired().HasMaxLength(70);
            builder.Property(x => x.FilterViewName).IsRequired().HasMaxLength(70);
            builder.Property(x => x.ResultViewName).IsRequired().HasMaxLength(70);

            builder.Property(x => x.Description).HasMaxLength(200);
            builder.Property(x => x.IconClass).HasMaxLength(50);

            builder.HasOne(x => x.Category).WithMany(x => x.Reports).HasForeignKey(x => x.CategoryId);


            //Seed Data
            builder.HasData(
                new Report { Id = 100, CategoryId = 1, ReportKey = "Customers_List", ReportName = "Customers List", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi bi-people", SortOrder = 1 },
                new Report { Id = 101, CategoryId = 1, ReportKey = "Rate_Requests", ReportName = "Rate Requests", ReportLocalName = "", ControllerName = "Reports", ActionName = "RateRequestReport", FilterViewName = "RateRequestsListFilter", ResultViewName = "RateRequestsListResult", IconClass = "bi bi-currency-dollar", SortOrder = 2 },


                new Report { Id = 200, CategoryId = 2, ReportKey = "Jobs_List", ReportName = "Jobs List", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi bi-briefcase", SortOrder = 1 },
                new Report { Id = 201, CategoryId = 2, ReportKey = "Bookings_List", ReportName = "Bookings List", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "RateRequestsListFilter", ResultViewName = "RateRequestsListResult", IconClass = "bi bi-calendar-check", SortOrder = 2 },
                new Report { Id = 202, CategoryId = 2, ReportKey = "BLs_List", ReportName = "BLs List", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "RateRequestsListFilter", ResultViewName = "RateRequestsListResult", IconClass = "bi bi-file-earmark-text", SortOrder = 3 },

                new Report { Id = 300, CategoryId = 3, ReportKey = "Containers_List", ReportName = "Containers List", ReportLocalName = "", ControllerName = "Reports", ActionName = "ShowContainersReport", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi bi-box-seam", SortOrder = 1 },
                new Report { Id = 301, CategoryId = 3, ReportKey = "Movements_Report", ReportName = "Movements Report", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "RateRequestsListFilter", ResultViewName = "RateRequestsListResult", IconClass = "bi  bi-arrow-left-right", SortOrder = 2 },

                new Report { Id = 400, CategoryId = 4, ReportKey = "Invoice_List", ReportName = "Invoice List", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi bi-receipt-cutoff", SortOrder = 1 },
                new Report { Id = 401, CategoryId = 4, ReportKey = "PaymentDocument_List", ReportName = "Payment Documents List", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi bi-box-seam", SortOrder = 2 },
                new Report { Id = 402, CategoryId = 4, ReportKey = "Cheques_Report", ReportName = "Cheques List", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "RateRequestsListFilter", ResultViewName = "RateRequestsListResult", IconClass = "bi  bi-arrow-left-right", SortOrder = 3 },
                new Report { Id = 403, CategoryId = 4, ReportKey = "Settlements_Report", ReportName = "Settlements Report", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "RateRequestsListFilter", ResultViewName = "RateRequestsListResult", IconClass = "bi  bi-arrow-left-right", SortOrder = 4 },

                new Report { Id = 500, CategoryId = 5, ReportKey = "KPI_Report", ReportName = "KPIs Report", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi bi-clipboard-data", SortOrder = 1 },
                new Report { Id = 501, CategoryId = 5, ReportKey = "Trend_Report", ReportName = "Executive Dashboard", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi  bi-speedometer2", SortOrder = 2 },
                new Report { Id = 502, CategoryId = 5, ReportKey = "Perfrmance_Report", ReportName = "Perfrmance Report", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi bi-bar-chart-line", SortOrder = 3 },
                new Report { Id = 503, CategoryId = 5, ReportKey = "Revenue_Analysis", ReportName = "Revenue Analysis", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi bi-cash-stack", SortOrder = 4 },
                new Report { Id = 504, CategoryId = 5, ReportKey = "Profitability_Report", ReportName = "Profitability Report", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi bi-graph-up-arrow", SortOrder = 5 },
                new Report { Id = 505, CategoryId = 5, ReportKey = "Company_wise_Performance", ReportName = "Company-wise Performance", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi bi-building", SortOrder = 6 },
                new Report { Id = 506, CategoryId = 5, ReportKey = "Customer_Performance", ReportName = "Customer Performance", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi bi-people", SortOrder = 7 },
                new Report { Id = 507, CategoryId = 5, ReportKey = "Activity_Summary", ReportName = "Activity Summary", ReportLocalName = "", ControllerName = "Reports", ActionName = "Filter", FilterViewName = "CustomersListFilter", ResultViewName = "CustomersListResult", IconClass = "bi bi-journal-text", SortOrder = 8 }


            );


        }
    }

    public class ReportCompanyRoleTypeConfiguration : IEntityTypeConfiguration<ReportCompanyRoleType>
    {
        public void Configure(EntityTypeBuilder<ReportCompanyRoleType> builder)
        {
            builder.ToTable("ReportCompanyRoleTypes", "rpt");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.CompanyRoleType).WithMany().HasForeignKey(x => x.CompanyRoleTypeId);
            builder.HasOne(x => x.Report).WithMany(x => x.ReportCompanyRoleTypes).HasForeignKey(x => x.ReportId);
        }
    }
}
