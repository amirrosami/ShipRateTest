using Dapper;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;
using ShipRate.Application.ViewModels.BookingConsoleVms;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Infrastructure.Data;
using System.Data;
using System.Drawing;

namespace ShipRate.Application.Services.JobsServices
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICargoDetailsService _cargoDetailsService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;


        public BookingService(ApplicationDbContext context, ICurrentUserService currentUserService, ICargoDetailsService cargoDetailsService, IConfiguration configuration, IWebHostEnvironment env)
        {
            _db = context;
            _currentUserService = currentUserService;
            _cargoDetailsService = cargoDetailsService;
            _configuration = configuration;
            _env = env;
        }
        public async Task<BookingFormVm> GetBookingForEditAsync(Guid bookingId)
        {
            var booking = await _db.Set<Booking>()
                .Include(b => b.Charges)
                .Include(b => b.VendorParty)
                .Include(b => b.Carrier)
                    .ThenInclude(b => b.Person)
                .Include(b => b.Origin)
                .Include(b => b.POL)
                .Include(b => b.Dest)
                .Include(b => b.TranshipPort)
                .Include(b => b.POD)
                .Include(b => b.BookingAgentParty)
                .Include(b => b.BookingLegs)
                .ThenInclude(x => x.FromLocation)
                .Include(b => b.BookingLegs)
                .ThenInclude(x => x.ToLocation)
                .Include(b => b.BookingContainerTypes)
                .Include(b => b.BookingParties)
                .Include(b => b.SeaDetail).ThenInclude(s => s.Voyages)
                .Include(b => b.RoadDetails)
                .Include(b => b.RailDetails)
                .Include(b => b.AirDetail).ThenInclude(a => a.Flights)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null)
                throw new Exception("Booking not found.");

            var vm = new BookingFormVm
            {
                Id = booking.Id,
                JobId = booking.JobId,
                CompanyId = booking.CompanyId,
                CompanyRoleTypeId = booking.CompanyRoleTypeId,
                BookingNo = booking.BookingNo,
                LineBookingNo = booking.LineBookingNo,
                LoadTypeId = booking.LoadTypeId,
                ServiceTypeId = booking.ServiceTypeId,
                TransportTypeId = booking.TransportTypeId,
                RateRequestId = booking.RateRequestId,
                BuyQuoteId = booking.BuyQuoteId,
                ContractId = booking.ContractId,
                CarrierPartyId = booking.CarrierPartyId,
                CarrierPartyName = booking.Carrier?.Person.PersonName,
                VendorPartyId = booking.VendorPartyId,
                VendorPartyName = booking.VendorParty?.PersonName,
                BookingAgentPartyId = booking.BookingAgentPartyId,
                BookingAgentPartyName = booking.BookingAgentParty?.PersonName,
                OriginId = booking.OriginId,
                OriginName = booking.Origin?.LocationName,
                DestId = booking.DestId,
                DestName = booking.Dest?.LocationName,
                TranshipPortId = booking.TranshipPortId,
                TranshipPortName = booking.TranshipPort?.LocationName,
                POLId = booking.POLId,
                POLName = booking.POL?.LocationName,
                PODId = booking.PODId,
                PODName = booking.POD?.LocationName,
                CargoReadyDate = booking.CargoReadyDate,
                CutOffDate = booking.CutOffDate,
                SICutOffDate = booking.SICutOffDate,
                VGMCutOffDate = booking.VGMCutOffDate,
                DocumentCutOffDate = booking.DocumentCutOffDate,
                ParentBookingId = booking.ParentBookingId,
                BookingStatusId = booking.BookingStatusId,
                Remarks = booking.Remarks
            };

            // نگاشت Legs
            vm.BookingLegs = booking.BookingLegs.Select(l => new BookingLegVm
            {
                Id = l.Id,
                ShipmentLegId = l.ShipmentLegId == null ? null : l.ShipmentLegId.Value,
                SequenceNo = l.SequenceNo,
                TransportTypeId = l.TransportTypeId,
                FromLocationId = l.FromLocationId,
                ToLocationId = l.ToLocationId,

                FromLocationName = l.FromLocation?.LocationName,
                ToLocationName = l.ToLocation?.LocationName,

                PlannedETD = l.PlannedETD,
                PlannedETA = l.PlannedETA,
                Remarks = l.Remarks
            }).OrderBy(l => l.SequenceNo).ToList();

            vm.BookingContainerTypes = booking.BookingContainerTypes.Select(c => new BookingContainerTypeVm
            {
                Id = c.Id,
                ContainerSizeTypeId = c.ContainerSizeTypeId,
                CntOwnTypeId = c.CntOwnTypeId,
                ContainerCount = c.ContainerCount,
                OverLengthCM = c.OverLengthCM,
                OverWidthCM = c.OverWidthCM,
                OverHeightCM = c.OverHeightCM
            }).ToList();

            vm.BookingParties = booking.BookingParties.Select(p => new BookingPartyVm
            {
                Id = p.Id,
                PartyRoleTypeId = p.PartyRoleTypeId,
                PartyId = p.PartyId
            }).ToList();

            if (booking.SeaDetail != null)
            {
                vm.SeaDetail = new BookingSeaDetailVm
                {
                    Id = booking.SeaDetail.Id,
                    ShippingLineId = booking.SeaDetail.ShippingLineId,
                    ServiceName = booking.SeaDetail.ServiceName,
                    CYCutOffDate = booking.SeaDetail.CYCutOffDate,
                    SICutOffDate = booking.SeaDetail.SICutOffDate,
                    DOCutOffDate = booking.SeaDetail.DOCutOffDate,
                    VGMCutOffDate = booking.SeaDetail.VGMCutOffDate,
                    ETD = booking.SeaDetail.ETD,
                    ETA = booking.SeaDetail.ETA,
                    EmptyPickupDepotId = booking.SeaDetail.EmptyPickupDepotId,
                    EmptyReturnDepotId = booking.SeaDetail.EmptyReturnDepotId,
                    ReceivingTerminalId = booking.SeaDetail.ReceivingTerminalId,
                    Remarks = booking.SeaDetail.Remarks,
                    Voyages = booking.SeaDetail.Voyages.Select(v => new BookingSeaVoyageVm
                    {
                        Id = v.Id,
                        SequenceNo = v.SequenceNo,
                        FromPortId = v.FromPortId,
                        ToPortId = v.ToPortId,
                        VoyageId = v.VoyageId,
                        VesselId = v.VesselId,
                        VesselName = v.VesselName,
                        VoyageNo = v.VoyageNo,
                        ETD = v.ETD,
                        ATD = v.ATD,
                        ETA = v.ETA,
                        ATA = v.ATA,
                        IsTransshipment = v.IsTransshipment,
                        Remarks = v.Remarks
                    }).OrderBy(v => v.SequenceNo).ToList()
                };
            }

            vm.RoadDetails = booking.RoadDetails.Select(r => new BookingRoadDetailVm
            {
                Id = r.Id,
                BookingLegId = r.BookingLegId,
                PickupLocationId = r.PickupLocationId,
                DeliveryLocationId = r.DeliveryLocationId,
                PickupAddress = r.PickupAddress,
                DeliveryAddress = r.DeliveryAddress,
                TruckTypeId = r.TruckTypeId,
                TruckCount = r.TruckCount,
                DriverName = r.DriverName,
                DriverMobile = r.DriverMobile,
                TruckPlateNo = r.TruckPlateNo,
                ContainerPickupDepotId = r.ContainerPickupDepotId,
                ContainerReturnDepotId = r.ContainerReturnDepotId,
                PlannedLoadingDate = r.PlannedLoadingDate,
                PlannedUnloadingDate = r.PlannedUnloadingDate,
                RoadPermitNo = r.RoadPermitNo,
                CMRNo = r.CMRNo,
                Remarks = r.Remarks
            }).ToList();

            vm.RailDetails = booking.RailDetails.Select(r => new BookingRailDetailVm
            {
                Id = r.Id,
                BookingLegId = r.BookingLegId,
                OriginStationId = r.OriginStationId,
                DestinationStationId = r.DestinationStationId,
                RailOperatorId = r.RailOperatorId,
                TrainNo = r.TrainNo,
                DepartureDate = r.DepartureDate,
                ArrivalDate = r.ArrivalDate,
                RailWaybillNo = r.RailWaybillNo,
                Remarks = r.Remarks
            }).ToList();

            if (booking.AirDetail != null)
            {
                vm.AirDetail = new BookingAirDetailVm
                {
                    Id = booking.AirDetail.Id,
                    DepartureAirportId = booking.AirDetail.DepartureAirportId,
                    ArrivalAirportId = booking.AirDetail.ArrivalAirportId,
                    AirlineId = booking.AirDetail.AirlineId,
                    AirlineBookingNo = booking.AirDetail.AirlineBookingNo,
                    MAWBNo = booking.AirDetail.MAWBNo,
                    HAWBNo = booking.AirDetail.HAWBNo,
                    AirCutOffDate = booking.AirDetail.AirCutOffDate,
                    ETD = booking.AirDetail.ETD,
                    ETA = booking.AirDetail.ETA,
                    CargoTerminalId = booking.AirDetail.CargoTerminalId,
                    Remarks = booking.AirDetail.Remarks,
                    Flights = booking.AirDetail.Flights.Select(f => new BookingAirFlightVm
                    {
                        Id = f.Id,
                        SequenceNo = f.SequenceNo,
                        FromAirportId = f.FromAirportId,
                        ToAirportId = f.ToAirportId,
                        AirlineId = f.AirlineId,
                        FlightNo = f.FlightNo,
                        ETD = f.ETD,
                        ATD = f.ATD,
                        ETA = f.ETA,
                        ATA = f.ATA,
                        Remarks = f.Remarks
                    }).OrderBy(f => f.SequenceNo).ToList()
                };
            }
            vm.CargoItems = await _cargoDetailsService.GetListAsync(new ViewModels.CargoDetailFilterVm { BookingId = bookingId });
            vm.Charges = booking.Charges.Select(x =>
            new BookingChargeVm
            {
                ChargeCodeId = x.ChargeTypeId,
                CurrencyId = x.CurrencyId,
                ChargeSideId = x.DirectionId,
                Id = x.Id,
                PartyId = x.PartyId.Value,
                Amount = x.Amount,
                Qty = x.Quantity,
                UnitPrice = x.UnitRate,
                TaxPercent = x.TaxPercent,
                IsTaxable = x.IsTaxable,
                Remarks = x.Remarks,
            }).ToList();
            return vm;
        }
        public async Task<Guid> SaveBookingAsync(BookingFormVm model)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                Booking booking;
                bool isNew = !model.Id.HasValue || model.Id == Guid.Empty;

                if (isNew)
                {
                    booking = new Booking { Id = Guid.CreateVersion7() };
                    _db.Set<Booking>().Add(booking);
                }
                else
                {
                    booking = await _db.Set<Booking>()
                        .Include(b => b.BookingLegs)
                        .Include(b => b.BookingContainerTypes)
                        .Include(b => b.BookingParties)
                        .Include(b => b.SeaDetail).ThenInclude(s => s.Voyages)
                        .Include(b => b.RoadDetails)
                        .Include(b => b.RailDetails)
                        .Include(b => b.AirDetail).ThenInclude(a => a.Flights)
                        .FirstOrDefaultAsync(b => b.Id == model.Id.Value);

                    if (booking == null)
                        throw new Exception("Booking record not found for updating.");

                    _db.Set<BookingLeg>().RemoveRange(booking.BookingLegs);
                    _db.Set<BookingContainerType>().RemoveRange(booking.BookingContainerTypes);
                    _db.Set<BookingParty>().RemoveRange(booking.BookingParties);
                    _db.Set<BookingRoadDetail>().RemoveRange(booking.RoadDetails);
                    _db.Set<BookingRailDetail>().RemoveRange(booking.RailDetails);
                }

                booking.JobId = model.JobId!.Value;
                booking.CompanyId = model.CompanyId;
                booking.CompanyRoleTypeId = model.CompanyRoleTypeId;
                var newBookingNo = await GenerateNextBookingNoAsync(default);

                booking.BookingNo = string.IsNullOrWhiteSpace(model.BookingNo) ? newBookingNo : model.BookingNo;

                booking.LineBookingNo = model.LineBookingNo;
                booking.LoadTypeId = model.LoadTypeId;
                booking.ServiceTypeId = model.ServiceTypeId;
                booking.TransportTypeId = model.TransportTypeId;
                booking.RateRequestId = model.RateRequestId;
                booking.BuyQuoteId = model.BuyQuoteId;
                booking.ContractId = model.ContractId;
                booking.CarrierPartyId = model.CarrierPartyId;
                booking.VendorPartyId = model.VendorPartyId;
                booking.BookingAgentPartyId = model.BookingAgentPartyId;
                booking.OriginId = model.OriginId;
                booking.DestId = model.DestId;
                booking.TranshipPortId = model.TranshipPortId;
                booking.POLId = model.POLId;
                booking.PODId = model.PODId;
                booking.CargoReadyDate = model.CargoReadyDate;
                booking.CutOffDate = model.CutOffDate;
                booking.SICutOffDate = model.SICutOffDate;
                booking.VGMCutOffDate = model.VGMCutOffDate;
                booking.DocumentCutOffDate = model.DocumentCutOffDate;
                booking.ParentBookingId = model.ParentBookingId;
                booking.Remarks = model.Remarks;

                if (isNew)
                {
                    booking.BookingStatusId = 1;
                    booking.CreateUserId = _currentUserService.UserId;
                    booking.CreatedAt = DateTime.Now;
                }
                else
                {
                    booking.UpdatedAt = DateTime.Now;
                    booking.EditUserId = _currentUserService.UserId;
                }

                if (model.BookingLegs != null)
                {
                    foreach (var legVm in model.BookingLegs)
                    {
                        booking.BookingLegs.Add(new BookingLeg
                        {
                            Id = Guid.CreateVersion7(),
                            BookingId = booking.Id,
                            ShipmentLegId = legVm.ShipmentLegId,
                            SequenceNo = legVm.SequenceNo,
                            TransportTypeId = legVm.TransportTypeId,
                            FromLocationId = legVm.FromLocationId,
                            ToLocationId = legVm.ToLocationId,
                            PlannedETD = legVm.PlannedETD,
                            PlannedETA = legVm.PlannedETA,
                            Remarks = legVm.Remarks,
                            CreateUserId = _currentUserService.UserId,
                            CreatedAt = DateTime.Now,
                        });
                    }
                }

                if (model.BookingContainerTypes != null)
                {
                    foreach (var cVm in model.BookingContainerTypes)
                    {
                        booking.BookingContainerTypes.Add(new BookingContainerType
                        {
                            Id = Guid.CreateVersion7(),
                            BookingId = booking.Id,
                            ContainerSizeTypeId = cVm.ContainerSizeTypeId,
                            CntOwnTypeId = cVm.CntOwnTypeId,
                            ContainerCount = cVm.ContainerCount,
                            OverLengthCM = cVm.OverLengthCM,
                            OverWidthCM = cVm.OverWidthCM,
                            OverHeightCM = cVm.OverHeightCM,
                            CreatedAt = DateTime.Now,
                            CreateUserId = _currentUserService.UserId,
                        });
                    }
                }

                if (model.Charges != null)
                {
                    foreach (var c in model.Charges)
                    {
                        booking.Charges.Add(new Charge
                        {
                            Id = Guid.CreateVersion7(),
                            PartyId = c.PartyId,
                            BookingId = booking.Id,
                            ChargeTypeId = c.ChargeCodeId,
                            DirectionId = c.ChargeSideId,
                            CurrencyId = c.CurrencyId,
                            Quantity = c.Qty,
                            Amount = c.Amount,
                            UnitRate = c.UnitPrice,
                            CreatedAt = DateTime.Now,
                            CreateUserId = _currentUserService.UserId,
                            CompanyId = booking.CompanyId,
                            IsDeleted = false,
                            JobId = booking.JobId
                        });
                    }
                }

                if (model.BookingParties != null)
                {
                    foreach (var pVm in model.BookingParties)
                    {
                        booking.BookingParties.Add(new BookingParty
                        {
                            Id = Guid.CreateVersion7(),
                            BookingId = booking.Id,
                            PartyRoleTypeId = pVm.PartyRoleTypeId,
                            PartyId = pVm.PartyId,
                            CreatedAt = DateTime.Now,
                            CreateUserId = _currentUserService.UserId,
                        });
                    }
                }

                if (model.TransportTypeId == 1 && model.SeaDetail != null)
                {
                    if (booking.SeaDetail == null)
                    {
                        booking.SeaDetail = new BookingSeaDetail
                        {
                            Id = Guid.CreateVersion7(),
                            BookingId = booking.Id,
                            CreatedAt = DateTime.Now,
                            CreateUserId = _currentUserService.UserId
                        };
                    }
                    else
                    {
                        _db.Set<BookingSeaVoyage>().RemoveRange(booking.SeaDetail.Voyages);
                        booking.SeaDetail.Voyages.Clear();
                        booking.SeaDetail.UpdatedAt = DateTime.Now;
                        booking.SeaDetail.EditUserId = _currentUserService.UserId;
                    }

                    booking.SeaDetail.ShippingLineId = model.SeaDetail.ShippingLineId;
                    booking.SeaDetail.ServiceName = model.SeaDetail.ServiceName;
                    booking.SeaDetail.CYCutOffDate = model.SeaDetail.CYCutOffDate;
                    booking.SeaDetail.SICutOffDate = model.SeaDetail.SICutOffDate;
                    booking.SeaDetail.DOCutOffDate = model.SeaDetail.DOCutOffDate;
                    booking.SeaDetail.VGMCutOffDate = model.SeaDetail.VGMCutOffDate;
                    booking.SeaDetail.ETD = model.SeaDetail.ETD;
                    booking.SeaDetail.ETA = model.SeaDetail.ETA;
                    booking.SeaDetail.EmptyPickupDepotId = model.SeaDetail.EmptyPickupDepotId;
                    booking.SeaDetail.EmptyReturnDepotId = model.SeaDetail.EmptyReturnDepotId;
                    booking.SeaDetail.ReceivingTerminalId = model.SeaDetail.ReceivingTerminalId;
                    booking.SeaDetail.Remarks = model.SeaDetail.Remarks;

                    if (model.SeaDetail.Voyages != null)
                    {
                        foreach (var vVm in model.SeaDetail.Voyages)
                        {
                            booking.SeaDetail.Voyages.Add(new BookingSeaVoyage
                            {
                                Id = Guid.CreateVersion7(),
                                BookingSeaDetailId = booking.SeaDetail.Id,
                                SequenceNo = vVm.SequenceNo,
                                FromPortId = vVm.FromPortId,
                                ToPortId = vVm.ToPortId,
                                VoyageId = vVm.VoyageId,
                                VesselId = vVm.VesselId,
                                VesselName = vVm.VesselName,
                                VoyageNo = vVm.VoyageNo,
                                ETD = vVm.ETD,
                                ATD = vVm.ATD,
                                ETA = vVm.ETA,
                                ATA = vVm.ATA,
                                IsTransshipment = vVm.IsTransshipment,
                                Remarks = vVm.Remarks,
                                CreatedAt = DateTime.Now,
                                CreateUserId = _currentUserService.UserId,
                            });
                        }
                    }
                }
                else if (booking.SeaDetail != null)
                {
                    _db.Set<BookingSeaVoyage>().RemoveRange(booking.SeaDetail.Voyages);
                    _db.Set<BookingSeaDetail>().Remove(booking.SeaDetail);
                    booking.SeaDetail = null;
                }

                if (model.TransportTypeId == 2 && model.RoadDetails != null)
                {
                    foreach (var rVm in model.RoadDetails)
                    {
                        booking.RoadDetails.Add(new BookingRoadDetail
                        {
                            Id = Guid.CreateVersion7(),
                            BookingId = booking.Id,
                            BookingLegId = rVm.BookingLegId,
                            PickupLocationId = rVm.PickupLocationId,
                            DeliveryLocationId = rVm.DeliveryLocationId,
                            PickupAddress = rVm.PickupAddress,
                            DeliveryAddress = rVm.DeliveryAddress,
                            TruckTypeId = rVm.TruckTypeId,
                            TruckCount = rVm.TruckCount,
                            DriverName = rVm.DriverName,
                            DriverMobile = rVm.DriverMobile,
                            TruckPlateNo = rVm.TruckPlateNo,
                            ContainerPickupDepotId = rVm.ContainerPickupDepotId,
                            ContainerReturnDepotId = rVm.ContainerReturnDepotId,
                            PlannedLoadingDate = rVm.PlannedLoadingDate,
                            PlannedUnloadingDate = rVm.PlannedUnloadingDate,
                            RoadPermitNo = rVm.RoadPermitNo,
                            CMRNo = rVm.CMRNo,
                            Remarks = rVm.Remarks,
                            CreatedAt = DateTime.Now,
                            CreateUserId = _currentUserService.UserId,
                        });
                    }
                }

                if (model.TransportTypeId == 3 && model.RailDetails != null)
                {
                    foreach (var rVm in model.RailDetails)
                    {
                        booking.RailDetails.Add(new BookingRailDetail
                        {
                            Id = Guid.CreateVersion7(),
                            BookingId = booking.Id,
                            BookingLegId = rVm.BookingLegId,
                            OriginStationId = rVm.OriginStationId,
                            DestinationStationId = rVm.DestinationStationId,
                            RailOperatorId = rVm.RailOperatorId,
                            TrainNo = rVm.TrainNo,
                            DepartureDate = rVm.DepartureDate,
                            ArrivalDate = rVm.ArrivalDate,
                            RailWaybillNo = rVm.RailWaybillNo,
                            Remarks = rVm.Remarks,
                            CreatedAt = DateTime.Now,
                            CreateUserId = _currentUserService.UserId,
                        });
                    }
                }

                if (model.TransportTypeId == 4 && model.AirDetail != null)
                {
                    if (booking.AirDetail == null)
                    {
                        booking.AirDetail = new BookingAirDetail
                        {
                            Id = Guid.CreateVersion7(),
                            BookingId = booking.Id,
                            CreatedAt = DateTime.Now,
                            CreateUserId = _currentUserService.UserId
                        };
                    }
                    else
                    {
                        _db.Set<BookingAirFlight>().RemoveRange(booking.AirDetail.Flights);
                        booking.AirDetail.Flights.Clear();
                        booking.AirDetail.UpdatedAt = DateTime.Now;
                        booking.AirDetail.EditUserId = _currentUserService.UserId;
                    }

                    booking.AirDetail.DepartureAirportId = model.AirDetail.DepartureAirportId;
                    booking.AirDetail.ArrivalAirportId = model.AirDetail.ArrivalAirportId;
                    booking.AirDetail.AirlineId = model.AirDetail.AirlineId;
                    booking.AirDetail.AirlineBookingNo = model.AirDetail.AirlineBookingNo;
                    booking.AirDetail.MAWBNo = model.AirDetail.MAWBNo;
                    booking.AirDetail.HAWBNo = model.AirDetail.HAWBNo;
                    booking.AirDetail.AirCutOffDate = model.AirDetail.AirCutOffDate;
                    booking.AirDetail.ETD = model.AirDetail.ETD;
                    booking.AirDetail.ETA = model.AirDetail.ETA;
                    booking.AirDetail.CargoTerminalId = model.AirDetail.CargoTerminalId;
                    booking.AirDetail.Remarks = model.AirDetail.Remarks;

                    if (model.AirDetail.Flights != null)
                    {
                        foreach (var fVm in model.AirDetail.Flights)
                        {
                            booking.AirDetail.Flights.Add(new BookingAirFlight
                            {
                                Id = Guid.CreateVersion7(),
                                BookingAirDetailId = booking.AirDetail.Id,
                                SequenceNo = fVm.SequenceNo,
                                FromAirportId = fVm.FromAirportId,
                                ToAirportId = fVm.ToAirportId,
                                AirlineId = fVm.AirlineId,
                                FlightNo = fVm.FlightNo,
                                ETD = fVm.ETD,
                                ATD = fVm.ATD,
                                ETA = fVm.ETA,
                                ATA = fVm.ATA,
                                Remarks = fVm.Remarks,
                                CreatedAt = DateTime.Now,
                                CreateUserId = _currentUserService.UserId,
                            });
                        }
                    }
                }
                else if (booking.AirDetail != null)
                {
                    _db.Set<BookingAirFlight>().RemoveRange(booking.AirDetail.Flights);
                    _db.Set<BookingAirDetail>().Remove(booking.AirDetail);
                    booking.AirDetail = null;
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return booking.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<byte[]> PrintRTSInstructionLetterPdfAsync(Guid id)
        {
            var connectionString = _configuration.GetConnectionString("ShipRateConnection");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync(
                "rpt.spRTSInstructionLetterReport",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            var headerRows = (await multi.ReadAsync()).ToList();
            var containerRows = (await multi.ReadAsync()).ToList();

            if (!headerRows.Any())
                return null;

            var dataSet = new DataSet("RTSData");

            var headerTable = new DataTable("Header");
            headerTable.Columns.Add("Id", typeof(Guid));
            headerTable.Columns.Add("RTSInstructionNo", typeof(string));
            headerTable.Columns.Add("RTSInstructionDate", typeof(DateTime));
            headerTable.Columns.Add("RTSInstructionLocalDate", typeof(string));
            headerTable.Columns.Add("CarrierName", typeof(string));
            headerTable.Columns.Add("CarrierLocalName", typeof(string));
            headerTable.Columns.Add("ShipperName", typeof(string));
            headerTable.Columns.Add("ShipperLocalName", typeof(string));

            headerTable.Columns.Add("BookingNo", typeof(string));
            headerTable.Columns.Add("OriginName", typeof(string));
            headerTable.Columns.Add("POLName", typeof(string));
            headerTable.Columns.Add("PODName", typeof(string));
            headerTable.Columns.Add("DestName", typeof(string));
            headerTable.Columns.Add("DepotName", typeof(string));
            headerTable.Columns.Add("Remarks", typeof(string));
            headerTable.Columns.Add("ReportFooterText", typeof(string));
            headerTable.Columns.Add("PageFooterText", typeof(string));

            foreach (var item in headerRows)
            {
                headerTable.Rows.Add(
                    item.Id,
                    item.RTSInstructionNo,
                    item.RTSInstructionDate ?? (object)DBNull.Value,
                    item.RTSInstructionLocalDate ?? string.Empty,
                    item.CarrierName ?? string.Empty,
                    item.CarrierLocalName ?? string.Empty,
                    item.ShipperName ?? string.Empty,
                    item.ShipperLocalName ?? string.Empty,
                    item.BookingNo ?? string.Empty,
                    item.OriginName ?? string.Empty,
                    item.POLName ?? string.Empty,
                    item.PODName ?? string.Empty,
                    item.DestName ?? string.Empty,
                    item.DepotName ?? string.Empty,
                    item.Remarks ?? string.Empty,
                    item.ReportFooterText,
                    "کاربر :" + await _currentUserService.GetUserFullNameAsync()
                );
            }

            var containersTable = new DataTable("Containers");
            containersTable.Columns.Add("Id", typeof(Guid));
            containersTable.Columns.Add("RTSInstructionId", typeof(Guid));
            containersTable.Columns.Add("ContainerSizeTypeName", typeof(string));
            containersTable.Columns.Add("ContainerCounts", typeof(int));
            containersTable.Columns.Add("Remarks", typeof(string));

            foreach (var item in containerRows)
            {
                containersTable.Rows.Add(
                    item.Id,
                    item.RTSInstructionId,
                    item.ContainerSizeTypeName ?? string.Empty,
                    item.ContainerCounts,
                    item.Remarks ?? string.Empty
                );
            }

            dataSet.Tables.Add(headerTable);
            dataSet.Tables.Add(containersTable);

            var reportPath = Path.Combine(_env.ContentRootPath, "Reports", "Booking", "RTSInstructionLetter.frx");

            using var report = new Report();
            report.Load(reportPath);
            report.RegisterData(dataSet, "RTSData");

            report.GetDataSource("Header").Enabled = true;
            report.GetDataSource("Containers").Enabled = true;



            var logoPath = Path.Combine(
                   _env.WebRootPath,
                   "logos",
                   "logo-1.png");

            var logoObject = report.FindObject("CompanyLogo") as PictureObject;

            if (logoObject is not null &&
                System.IO.File.Exists(logoPath))
            {
                using var sourceImage = Image.FromFile(logoPath);
                logoObject.Image = new Bitmap(sourceImage);
            }




            report.Prepare();

            using var pdfExport = new PDFSimpleExport();
            using var stream = new MemoryStream();

            report.Export(pdfExport, stream);
            return stream.ToArray();
        }

        public async Task<BookingConsoleBookingSummaryVm?> GetBookingSummaryAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            var vm = await _db.Bookings
                .AsNoTracking()
                .Where(x => x.Id == bookingId && !x.IsDeleted)
                .Select(x => new BookingConsoleBookingSummaryVm
                {
                    BookingId = x.Id,
                    BookingNo = x.BookingNo,
                    JobId = x.JobId,
                    JobFileNo = x.Job.JobFileNumber,
                    SalesUserName = x.Job.SalesPerson != null ? x.Job.SalesPerson.UserName : "",
                    CarrierName = x.Carrier != null ? x.Carrier.Person.PersonName : string.Empty,
                    POL = x.POL != null ? x.POL.LocationName : string.Empty,
                    POD = x.POD != null ? x.POD.LocationName : string.Empty,
                    GrossWeightKg = 0,
                    PackageCount = 0,
                    ContainerCount = x.BookingContainerTypes.Count(),
                    ShipperText = x.BookingParties.Where(x => x.PartyRoleTypeId == 53).Any() ? x.BookingParties.Where(x => x.PartyRoleTypeId == 53).FirstOrDefault().Party.PersonName : "",
                    BookerClientName = x.Job.Parties.Where(x => x.RoleTypeId == 31).Any() ? x.Job.Parties.Where(x => x.RoleTypeId == 31).FirstOrDefault().Party.PersonName : "",
                })
                .FirstOrDefaultAsync(cancellationToken);
            var containers = await _db.ContainerActions
                                    .Include(x => x.Container)
                                    .ThenInclude(x => x.ContainerSizeType)
                                    .Where(x => x.IsDeleted == false && x.BookingId == bookingId && x.CntActionTypeId == 1)
                                    .OrderBy(x => x.ContainerNumber)
                                    .Select(x => new BookingConsoleContainerVm
                                    {
                                        ContainerNo = x.ContainerNumber,
                                        ContainerType = x.Container.ContainerSizeType.ShortName,
                                        ContainerUseId = x.ContainerUseId,
                                        RTSDate = x.ContainerActionDateTime,
                                    })
                                    .ToListAsync();
            vm.Containers = containers;
            vm.ContainerCount = containers.Count;
            return vm;
        }
        public async Task<BookingConsoleVm> GetBookingConsoleAsync(Guid id)
        {
            var vm = new BookingConsoleVm();
            vm.BookingId = id;
            vm.Summary = await GetBookingSummaryAsync(id, default);




            vm.JobId = vm.Summary.JobId;
            return vm;
        }

        public async Task<List<InvoiceRowVm>> GetInvoicesByBookingIdAsync(Guid bookingId, string filterType)
        {
            var query = _db.Invoices
                           .AsNoTracking()
                           .Where(i => i.BookingId == bookingId && i.InvoiceStatusId != 8);
            switch (filterType)
            {
                case "BuyF":
                    query = query.Where(x => x.FinanceDirectionId == 2 && x.InvoiceFNFId == 1 && x.InvoiceModelId == 1);
                    break;
                case "BuyNF":
                    query = query.Where(x => x.FinanceDirectionId == 2 && x.InvoiceFNFId == 2 && x.InvoiceModelId == 1);
                    break;
                case "SellF":
                    query = query.Where(x => x.FinanceDirectionId == 1 && x.InvoiceFNFId == 1 && x.InvoiceModelId == 1);
                    break;
                case "SellNF":
                    query = query.Where(x => x.FinanceDirectionId == 1 && x.InvoiceFNFId == 2 && x.InvoiceModelId == 1);
                    break;
                case "PreInvoices":
                    query = query.Where(x => x.InvoiceModelId == 2);
                    break;
                default:
                    break;
            }

            return await query
                .OrderBy(x => x.InvoiceModelId).ThenByDescending(x => x.FinanceDirectionId).ThenByDescending(x => x.InvoiceFNFId)
                .Select(i => new InvoiceRowVm
                {
                    InvoiceId = i.Id,
                    InvoiceNo = i.InvoiceNo,
                    InvoiceDate = i.InvoiceDate.Value,
                    PartyName = i.Party != null ? (i.Party.PersonName) : "Unknown",
                    TypeName = i.InvoiceType.InvoiceTypeName,
                    TotalAmount = i.TotalAmount,
                    CurrencyCode = i.Currency.CurrencyCode,
                    StatusName = i.InvoiceStatus.InvoiceStatusName,
                    StatusId = i.InvoiceStatusId,
                    IsPosted = i.IsPostedToAcc,
                    FNFName = i.InvoiceFNF.FNFName,
                    InvocieModelName = i.InvoiceModel.InvoiceModelName,
                    CompanyId = i.CompanyId,
                    CompanyName = i.Company.CompanyAbb,
                    DirectionName = i.FinanceDirection.InvoiceContextName,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    CreateUserName = i.CreateUser.UserName,
                    UpdateUserName = i.UpdateUser.UserName,
                    SettledAmount = i.SettledAmount,
                    RemainingAmount = i.RemainingAmount,
                    SubjectName = i.InvoiceSubject.InvoiceSubjectName,
                    Serial = i.Serial,
                    SerialPrefix = i.SerialPrefix,
                    InvoiceModelId = i.InvoiceModelId,
                    DirectionId = i.FinanceDirectionId
                })
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<BookingConsoleDetentionsVm?> GetDetentionsByBookingAsync(Guid bookingId, CancellationToken cancellationToken)
        {
            var items = await _db.Detentions
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.MainCurrency)
                .Include(x => x.PaymentCurrency)
                .Include(x => x.Company)
                .Include(x => x.FinanceDirection)
                .Include(x => x.Party)
                .Include(x => x.DetentionStatus)
                .Include(x => x.CreateUser)
                .Include(x => x.EditUser)
                .Where(x => x.BookingId == bookingId)
                .OrderByDescending(x => x.DetentionToDate)
                .Select(x => new DetentionRowVm
                {
                    Id = x.Id,
                    DetentionNo = x.DetentionNo,
                    DirectionName = x.FinanceDirection.InvoiceContextName,
                    CompanyName = x.Company.CompanyAbb,
                    IssueDate = x.CreatedAt,
                    StatusId = x.DetentionStatusId,
                    PartyName = x.Party != null ? x.Party.PersonName : null,
                    StatusTitle = x.DetentionStatus != null ? x.DetentionStatus.DetentionStatusName : null,
                    GrossAmount = x.GrossAmount,
                    VATAmount = x.VATAmount,
                    Amount = x.Amount,
                    DetentionToDate = x.DetentionToDate,
                    IsFinal = x.IsFinal,
                    Remarks = x.Remarks,
                    TariffTitle = x.TariffBook.Name,
                    FreeDays = x.Items.Max(x => x.DetentionItemFreeDays),
                    CreateUserName = x.CreateUser.FullName,
                    IssueDatePersian = PersianDateHelper.ToShamsi(x.CreatedAt),
                    FinalPayableAmount = x.FinalPayableAmount,
                    FinalCurrencyName = x.PaymentCurrency.CurrencyCode,
                    MainCurrencyName = x.MainCurrency.CurrencyCode,
                    Settled = x.SettledAmount,
                    Remain = x.RemainingAmount
                })
                .ToListAsync(cancellationToken);
            return new BookingConsoleDetentionsVm
            {
                BookingId = bookingId,
                BookingNo = "-",
                Items = items
            };
        }


        private async Task<string> GenerateNextBookingNoAsync(CancellationToken cancellationToken = default)
        {
            const string prefix = "BK-";
            var lastBookingNo = await _db.Bookings
                .AsNoTracking()
                .Where(b => b.BookingNo != null && b.BookingNo.StartsWith(prefix))
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => b.BookingNo)
                .FirstOrDefaultAsync(cancellationToken);

            long nextNumber = 1000;
            if (!string.IsNullOrWhiteSpace(lastBookingNo))
            {
                // استخراج بخش عددی بعد از پیشوند
                var numericPart = lastBookingNo.Substring(prefix.Length);
                if (long.TryParse(numericPart, out var lastNum))
                {
                    nextNumber = lastNum + 1;
                }
            }
            return $"{prefix}{nextNumber}";
        }



    }
}
