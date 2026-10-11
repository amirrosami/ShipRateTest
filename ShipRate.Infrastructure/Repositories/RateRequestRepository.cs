namespace ShipRate.Infrastructure.Repositories
{
    //public class RateRequestRepository : Repository<RateRequest>, IRateRequestRepository
    //{
    //    public RateRequestRepository(ApplicationDbContext context) : base(context)
    //    {
    //    }

    //    public async Task<RateRequest> GetByIdWithDetailsAsync(Guid id)
    //    {
    //        return await _dbSet
    //            .Include(r => r.TransportTypes)
    //            .Include(r => r.Packages)
    //            .Include(r => r.LCLLTLs)
    //            .Include(r => r.FTLs)
    //            .Include(r => r.FCLs)
    //            .Include(r => r.DeliveryOptions)
    //            .FirstOrDefaultAsync(r => r.Id == id);
    //    }

    //    public async Task<(IEnumerable<RateRequest> Items, int TotalCount)> SearchAsync(RateRequestSearchDto searchDto)
    //    {
    //        var query = _dbSet.AsQueryable();

    //        //// Apply filters
    //        //if (!string.IsNullOrWhiteSpace(searchDto.RateRequestNumber))
    //        //{
    //        //    query = query.Where(r => r.RateRequestNumber.Contains(searchDto.RateRequestNumber));
    //        //}

    //        if (searchDto.CustomerId.HasValue)
    //        {
    //            query = query.Where(r => r.CustomerId == searchDto.CustomerId.Value);
    //        }

    //        if (searchDto.OriginLocationId.HasValue)
    //        {
    //            query = query.Where(r => r.OriginId == searchDto.OriginLocationId.Value);
    //        }

    //        if (searchDto.DestinationLocationId.HasValue)
    //        {
    //            query = query.Where(r => r.DestinationLocationId == searchDto.DestinationLocationId.Value);
    //        }

    //        if (searchDto.CargoTypeId.HasValue)
    //        {
    //            query = query.Where(r => r.CargoTypeId == searchDto.CargoTypeId.Value);
    //        }

    //        if (searchDto.IncotermsId.HasValue)
    //        {
    //            query = query.Where(r => r.IncotermsId == searchDto.IncotermsId.Value);
    //        }

    //        if (searchDto.StatusId.HasValue)
    //        {
    //            query = query.Where(r => r.StatusId == searchDto.StatusId.Value);
    //        }

    //        if (searchDto.FromDate.HasValue)
    //        {
    //            query = query.Where(r => r.CreateDate >= searchDto.FromDate.Value);
    //        }

    //        if (searchDto.ToDate.HasValue)
    //        {
    //            query = query.Where(r => r.CreateDate <= searchDto.ToDate.Value);
    //        }

    //        if (searchDto.IsAir.HasValue)
    //        {
    //            query = query.Where(r => r.RateRequestTransportTypes.Any(t => t.TransportTypeId == 1) == searchDto.IsAir.Value);
    //        }

    //        if (searchDto.IsSea.HasValue)
    //        {
    //            query = query.Where(r => r.RateRequestTransportTypes.Any(t => t.TransportTypeId == 2) == searchDto.IsSea.Value);
    //        }

    //        if (searchDto.IsRoad.HasValue)
    //        {
    //            query = query.Where(r => r.RateRequestTransportTypes.Any(t => t.TransportTypeId == 3) == searchDto.IsRoad.Value);
    //        }

    //        if (searchDto.IsRail.HasValue)
    //        {
    //            query = query.Where(r => r.RateRequestTransportTypes.Any(t => t.TransportTypeId == 4) == searchDto.IsRail.Value);
    //        }

    //        // Get total count
    //        var totalCount = await query.CountAsync();

    //        // Apply sorting
    //        query = query.OrderByDescending(r => r.CreateDate);

    //        // Apply paging
    //        var items = await query
    //            .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
    //            .Take(searchDto.PageSize)
    //            .ToListAsync();

    //        return (items, totalCount);
    //    }
    //}
}
