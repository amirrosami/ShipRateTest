using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Infrastructure.Data;
using System.Collections;

namespace ShipRate.Application.Services.JobsServices
{
    public class JobLookupService : IJobLookupService
    {
        private readonly ApplicationDbContext _db;

        public JobLookupService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<SelectListItem>> GetCompaniesAsync()
        {
            var items = await
                (from c in _db.Companies
                 join p in _db.Persons on c.PersonId equals p.Id
                 where c.IsActive && !c.IsDeleted
                    && !p.IsDeleted
                 orderby c.Id
                 select new SelectListItem
                 {
                     Value = c.Id.ToString(),
                     Text = p.PersonName
                 })
                .ToListAsync();

            return items;
        }

        public async Task<List<SelectListItem>> GetCustomersAsync()
        {
            var items = await
                (from c in _db.Customers
                 join p in _db.Persons on c.Id equals p.Id
                 where !c.IsDeleted
                    && !p.IsDeleted
                 orderby p.PersonName
                 select new SelectListItem
                 {
                     Value = c.Id.ToString(),
                     Text = p.PersonName
                 })
                .ToListAsync();

            return items;
        }

        public async Task<List<SelectListItem>> GetLocationsAsync()
        {
            return await _db.Locations
                .Where(x => x.IsActive && !x.IsDeleted && x.LocationTypeId != 1)
                .OrderBy(x => x.LocationName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LocationName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetTransportTypesAsync()
        {
            return await _db.TransportTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TransportTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetTradeTypesAsync()
        {
            return await _db.TradeTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TradeTypeName
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetFullMTsAsync()
        {
            return await _db.FullMT
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullMTName
                })
                .ToListAsync();
        }


        public async Task<List<SelectListItem>> GetLoadTypesAsync()
        {
            return await _db.LoadTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LoadTypeCode
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCargoHandlingTypesAsync()
        {
            return await _db.CargoHandlingTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CargoHandlingTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCargoNaturesAsync()
        {
            return await _db.CargoNatures
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CargoNatureName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetIncotermsAsync()
        {
            return await _db.Incoterms
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = String.Format("{0}({1})", x.IncotermCode, x.IncoTermTitle)
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetPaymentTypesAsync()
        {
            return await _db.PaymentTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PaymentTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetJobTypesAsync()
        {
            return await _db.JobTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.JobTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetJobStatusesAsync()
        {
            return await _db.JobStatuses
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.JobStatusName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetMovementTypesAsync()
        {
            return await _db.MovementTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.MovementTypeCode
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetContainerSizeTypesAsync()
        {
            return await _db.ContainerSizeTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ShortName
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetPackageTypesAsync()
        {
            return await _db.PackageTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PackageTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDimUnitsAsync()
        {
            return await _db.MeasurementUnits
                .Where(x => x.IsActive && !x.IsDeleted && x.MeasurementUnitCategoryId == 3)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.MeasurementUnitName
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetWeightUnitsAsync()
        {
            return await _db.MeasurementUnits
                .Where(x => x.IsActive && !x.IsDeleted && x.MeasurementUnitCategoryId == 1)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.MeasurementUnitName
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetVolumeUnitsAsync()
        {
            return await _db.MeasurementUnits
                .Where(x => x.IsActive && !x.IsDeleted && x.MeasurementUnitCategoryId == 2)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.MeasurementUnitName
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetTempUnitsAsync()
        {
            return await _db.MeasurementUnits
                .Where(x => x.IsActive && !x.IsDeleted && x.MeasurementUnitCategoryId == 6)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.MeasurementUnitName
                })
                .ToListAsync();
        }


        public async Task<List<SelectListItem>> GetLoadTypesByTransportTypeAsync(int? transportTypeId)
        {
            var query = _db.TransportTypeLoadTypes
                .Include(x => x.LoadType)
                .Include(x => x.TransportType)
                .AsQueryable();

            if (transportTypeId.HasValue)
            {
                if (transportTypeId.Value == 5)   //Multimodal
                {
                }
                else query = query.Where(x => x.TransportTypeId == transportTypeId.Value);
            }
            else
            {
                query = query.Where(x => false); // یا همه را برگردان، بسته به نیاز
            }

            return await query
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LoadType.LoadTypeCode
                }).Distinct()
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetServiceTypesAsync()
        {
            return await _db.ServiceTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ServiceTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCompanyRoleTypesAsync()
        {
            return await _db.CompanyRoleTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyRoleTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetRoleTypesAsync()
        {
            return await _db.RoleTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.RoleTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.RoleTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetChargeTypesAsync()
        {
            return await _db.ChargeTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChargeTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCurrenciesAsync()
        {
            return await _db.Currencies
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CurrencyCode
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetJobSpecificPartiesAsync(Guid jobId)
        {
            return await _db.Persons
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PersonName
                })
                .ToListAsync();
        }


        public async Task<List<SelectListItem>> GetShippersAsync(Guid? bookingId)
        {
            var query = _db.Persons
                .AsNoTracking();

            if (bookingId != null && bookingId.HasValue && bookingId != Guid.Empty)
            {
                var bookingParty = await _db.BookingParties.Where(x => x.Id == bookingId && x.PartyRoleTypeId == 51).FirstOrDefaultAsync();
                if (bookingParty != null)
                {
                    query = query.Where(x => x.Id == bookingParty.PartyId);
                }
            }
            return await query
                .OrderBy(s => s.PersonName)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.PersonName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCarriersAsync(Guid? carrierId)
        {
            var query = _db.Carriers
                .Include(x => x.Person)
                .AsNoTracking();

            if (carrierId != null && carrierId.HasValue && carrierId != Guid.Empty)
            {
                query = query.Where(x => x.Id == carrierId);
            }

            return await query
                .OrderBy(c => c.Person.PersonName)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Person.PersonName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDepotsAsync()
        {
            return await _db.Locations
                .Where(x => x.LocationTypeId == 7)
                .OrderBy(d => d.LocationName)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.LocationName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetRTSStatusesAsync()
        {
            // اگر Statusها در دیتابیس هستند:
            return await _db.RTSInstructionStatus
                .OrderBy(s => s.Id)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.StatusName
                })
                .ToListAsync();
        }


        public async Task<List<SelectListItem>> GetCustomOfficesAsync()
        {
            return await _db.Locations
                .Where(x => x.IsActive && !x.IsDeleted && x.LocationTypeId == 5)
                .OrderBy(x => x.LocationName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LocationName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCntOwnTypesAsync()
        {
            return await _db.CntOwnTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CntOwnTypeName
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetShipmentLegsAsync(Guid jobId)
        {
            return await _db.ShipmentLegs
                .Include(x => x.Shipment)
                .ThenInclude(x => x.Job)
                .Include(x => x.LegType)
                .Include(x => x.FromLocation)
                .Include(x => x.ToLocation)
                .Where(x => !x.IsDeleted && x.Shipment.JobId == jobId)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = "Leg Seq: " + x.SequenceNo.ToString() + "-" + x.LegType.LegTypeName + " - From:" + x.FromLocation.LocationName + " To: " + x.ToLocation.LocationName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetJobSourceTypesAsync()
        {
            return await _db.JobSourceTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.JobSourceTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSalesUsersAsync()
        {
            return await _db.Users
                .Where(x => x.IsActive)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetCargoCommoditiesAsync()
        {
            return await _db.CargoCommodities
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.CargoCommodityName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CargoCommodityName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetTruckTypesAsync()
        {
            return await _db.TruckTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.TruckTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TruckTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetFlightsAsync()
        {
            return await _db.Flights
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.FlightNumber)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FlightNumber
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetFlightDatesAsync()
        {
            return await _db.FlightDates
                .Include(x => x.Flight)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Flight.FlightNumber)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Flight.FlightNumber + "-" + x.ETD.Value.ToString("yyyy-MM-dd")
                })
                .ToListAsync();
        }

        public async Task<IEnumerable> GetBookingStatusesAsync()
        {
            return await _db.BookingStatuses
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BookingStatusName
                })
                .ToListAsync();
        }
        public async Task<IEnumerable> GetPartiesAsync()
        {
            return await _db.Persons
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PersonName
                })
                .ToListAsync();
        }
        public async Task<IEnumerable> GetContainerOwnTypesAsync()
        {
            return await _db.CntOwnTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CntOwnTypeName
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetTerminalsAsync()
        {
            return await _db.Locations
                .Where(x => x.LocationTypeId == 6)
                .OrderBy(d => d.LocationName)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.LocationName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetVesselsAsync()
        {
            return await _db.Vessels
                .OrderBy(d => d.VesselName)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.VesselName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetVoyagesAsync()
        {
            return await _db.Voyages
                .OrderBy(d => d.VoyageNo)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.VoyageNo
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetAirportsAsync()
        {
            return await _db.Locations
                .Where(x => x.LocationTypeId == 3)
                .OrderBy(d => d.LocationName)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.LocationName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetAirlinesAsync()
        {
            return await _db.Carriers
                .Include(x => x.Person)
                .Where(x => x.CarrierTypeId == 3)
                .OrderBy(d => d.Person.PersonName)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Person.PersonName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetBLTypesByTransportTypeIdAsync(int? transportTypeId)
        {
            var allblTypes = await _db.BLTypes
                .OrderBy(d => d.Id)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.BLTypeName
                })
                .ToListAsync();
            if (transportTypeId != null && transportTypeId.Value != 0)
            {
                var blTypes = await _db.BLTypes
                    .Where(x => x.TransportTypeId == transportTypeId)
                    .OrderBy(d => d.Id)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.BLTypeName
                    })
                    .ToListAsync();
                if (blTypes == null || blTypes.Count == 0) return allblTypes;
                return blTypes;
            }
            return allblTypes;
        }

        public async Task<List<SelectListItem>> GetPersonsAsync()
        {
            return await _db.Persons
                .OrderBy(d => d.PersonName)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.PersonName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetDeclarationTypesAsync()
        {
            return await _db.CustomsDeclarationTypes
                .OrderBy(d => d.CustomsDeclarationTypeName)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.CustomsDeclarationTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetFinanceDirectionsForInvoiceContextAsync()
        {
            return await _db.FinanceDirections
                .OrderBy(d => d.Id)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.InvoiceContextName
                })
                .ToListAsync();
        }
    }
}
