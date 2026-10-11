using ShipRate.Application.Interfaces;

namespace ShipRate.Application.Services
{
    public class LookupService : ILookupService
    {
        //    private readonly ApplicationDbContext _context;

        //    public LookupService(ApplicationDbContext context)
        //    {
        //        _context = context;
        //    }

        //    public async Task<IEnumerable<LookupDto>> GetCargoHandlingTypesAsync()
        //    {
        //        // Assuming you have a CargoHandlingTypes table
        //        return await _context.Set<LookupDto>()
        //            .FromSqlRaw("SELECT Id AS Id, CargoHandlingTypeName AS Name FROM base.CargoHandlingTypes WHERE IsActive = 1")
        //            .Select(x => new LookupDto { Id = x.Id, Name = x.Name })
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<LookupDto>> GetPackageTypesAsync()
        //    {
        //        return await _context.Database
        //            .SqlQuery<LookupDto>($"SELECT Id, PackageTypeName AS Name FROM base.PackageTypes WHERE IsActive = 1")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<LookupDto>> GetTransportTypesAsync()
        //    {
        //        return await _context.Database
        //            .SqlQuery<LookupDto>($"SELECT Id, TransportTypeName AS Name FROM base.TransportTypes WHERE IsActive = 1")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<LookupDto>> GetIncotermsAsync()
        //    {
        //        return await _context.Database
        //            .SqlQuery<LookupDto>($"SELECT Id AS Id, IncotermsCode AS Name FROM base.Incoterms WHERE IsActive = 1")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<LookupDto>> GetStatusesAsync()
        //    {
        //        return await _context.Database
        //            .SqlQuery<LookupDto>($"SELECT Id, StatusName AS Name FROM dbo.Statuses WHERE IsActive = 1 AND StatusType = 'RateRequest'")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<LookupDto>> GetCurrenciesAsync()
        //    {
        //        return await _context.Database
        //            .SqlQuery<LookupDto>($"SELECT Id, CurrencyCode AS Name FROM dbo.Currencies WHERE IsActive = 1")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<LookupDto>> GetDimensionUnitsAsync()
        //    {
        //        return await _context.Database
        //            .SqlQuery<LookupDto>($"SELECT UnitId AS Id, UnitName AS Name FROM dbo.Units WHERE IsActive = 1 AND UnitType = 'Dimension'")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<LookupDto>> GetWeightUnitsAsync()
        //    {
        //        return await _context.Database
        //            .SqlQuery<LookupDto>($"SELECT UnitId AS Id, UnitName AS Name FROM dbo.Units WHERE IsActive = 1 AND UnitType = 'Weight'")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<LookupDto>> GetVolumeUnitsAsync()
        //    {
        //        return await _context.Database
        //            .SqlQuery<LookupDto>($"SELECT UnitId AS Id, UnitName AS Name FROM dbo.Units WHERE IsActive = 1 AND UnitType = 'Volume'")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<LookupDto>> GetTruckTypesAsync()
        //    {
        //        return await _context.Database
        //            .SqlQuery<LookupDto>($"SELECT TruckTypeId AS Id, TruckTypeName AS Name FROM dbo.TruckTypes WHERE IsActive = 1")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<LookupDto>> GetContainerTypesAsync()
        //    {
        //        return await _context.Database
        //            .SqlQuery<LookupDto>($"SELECT ContainerTypeId AS Id, ContainerTypeName AS Name FROM dbo.ContainerTypes WHERE IsActive = 1")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<LookupDto>> GetDeliveryOptionsAsync()
        //    {
        //        return await _context.Database
        //            .SqlQuery<LookupDto>($"SELECT DeliveryOptionId AS Id, DeliveryOptionName AS Name FROM dbo.DeliveryOptions WHERE IsActive = 1")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<CustomerLookupDto>> SearchCustomersAsync(string searchTerm)
        //    {
        //        if (string.IsNullOrWhiteSpace(searchTerm))
        //            return new List<CustomerLookupDto>();

        //        var query = @"
        //            SELECT TOP 20 
        //                CustomerId, 
        //                CustomerName, 
        //                CustomerCode, 
        //                Email, 
        //                Phone 
        //            FROM dbo.Customers 
        //            WHERE IsActive = 1 
        //                AND (CustomerName LIKE {0} OR CustomerCode LIKE {0} OR Email LIKE {0})
        //            ORDER BY CustomerName";

        //        return await _context.Database
        //            .SqlQuery<CustomerLookupDto>($"{query}", $"%{searchTerm}%")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<LocationLookupDto>> SearchLocationsAsync(string searchTerm)
        //    {
        //        if (string.IsNullOrWhiteSpace(searchTerm))
        //            return new List<LocationLookupDto>();

        //        var query = @"
        //            SELECT TOP 20 
        //                LocationId, 
        //                LocationName, 
        //                City, 
        //                Country, 
        //                PortCode 
        //            FROM dbo.Locations 
        //            WHERE IsActive = 1 
        //                AND (LocationName LIKE {0} OR City LIKE {0} OR Country LIKE {0} OR PortCode LIKE {0})
        //            ORDER BY LocationName";

        //        return await _context.Database
        //            .SqlQuery<LocationLookupDto>($"{query}", $"%{searchTerm}%")
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<CargoLookupDto>> SearchCargosAsync(string searchTerm)
        //    {
        //        if (string.IsNullOrWhiteSpace(searchTerm))
        //            return new List<CargoLookupDto>();

        //        var query = @"
        //            SELECT TOP 20 
        //                CargoTypeId, 
        //                CargoTypeName, 
        //                HSCode, 
        //                Description 
        //            FROM dbo.CargoTypes 
        //            WHERE IsActive = 1 
        //                AND (CargoTypeName LIKE {0} OR HSCode LIKE {0} OR Description LIKE {0})
        //            ORDER BY CargoTypeName";

        //        return await _context.Database
        //            .SqlQuery<CargoLookupDto>($"{query}", $"%{searchTerm}%")
        //            .ToListAsync();
        //    }
        //}
    }
}
