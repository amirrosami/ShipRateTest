using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces.Manifest;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Infrastructure.Data;
using System.Data;
using System.Drawing;


namespace ShipRate.Application.Services
{
    public sealed class BLManifestReportService : IBLManifestReportService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public BLManifestReportService(ApplicationDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }

        public async Task<BLManifestReportVm?> GetAsync(
            Guid blId,
            CancellationToken cancellationToken = default)
        {
            var bl = await _dbContext.BLs
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.Id == blId && !x.IsDeleted)

                .Include(x => x.Company)
                .Include(x => x.Carrier)
                    .ThenInclude(x => x.Person)
                .Include(x => x.IssuerParty)
                .Include(x => x.PaymentType)
                .Include(x => x.LoadType)
                .Include(x => x.MovementType)
                .Include(x => x.Incoterm)
                .Include(x => x.TransportType)
                .Include(x => x.BLType)
                .Include(x => x.BLLevel)
                .Include(x => x.PackageType)

                .Include(x => x.BLParties)
                    .ThenInclude(x => x.Party)
                .Include(x => x.BLParties)
                    .ThenInclude(x => x.PartyRoleType)

                .Include(x => x.BLVoyages)
                    .ThenInclude(x => x.VoyageType)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.ContainerSizeType)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.LoadType)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.FullMT)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CntOwnType)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.DimensionUnit)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CargoAllocations)
                        .ThenInclude(x => x.CargoDetail)
                            .ThenInclude(x => x.PackageType)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CargoAllocations)
                        .ThenInclude(x => x.CargoDetail)
                            .ThenInclude(x => x.DGClass)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CargoAllocations)
                        .ThenInclude(x => x.CargoDetail)
                            .ThenInclude(x => x.DGPackingGroup)

                .Include(x => x.CargoDetails)
                    .ThenInclude(x => x.PackageType)

                .FirstOrDefaultAsync(cancellationToken);

            if (bl is null)
            {
                return null;
            }

            var shipper = bl.BLParties
                .FirstOrDefault(x => x.RoleTypeId == BLPartyRoleTypeIds.Shipper);

            var consignee = bl.BLParties
                .FirstOrDefault(x => x.RoleTypeId == BLPartyRoleTypeIds.Consignee);

            var notifyParty = bl.BLParties
                .FirstOrDefault(x => x.RoleTypeId == BLPartyRoleTypeIds.NotifyParty);

            //var containerIdsWithAllocatedCargo = bl.BLContainers
            //    .SelectMany(x => x.CargoAllocations)
            //    .Select(x => x.CargoDetailId)
            //    .Distinct()
            //    .ToHashSet();

            //var containers = bl.BLContainers
            //    .OrderBy(x => x.ContainerNumber)
            //    .Select((container, containerIndex) => new BLManifestContainerVm
            //    {
            //        RowNo = containerIndex + 1,

            //        ContainerNumber = container.ContainerNumber,

            //        // اگر property نام متفاوتی دارد، .Name را تغییر دهید.
            //        ContainerSizeType = container.ContainerSizeType?.ShortName,
            //        LoadType = container.LoadType?.LoadTypeCode,
            //        FullMT = container.FullMT?.FullMTName,
            //        OwnershipType = container.CntOwnType?.CntOwnTypeName,

            //        SealNumber = container.SealNumber,
            //        SealNumber2 = container.SealNumber2,

            //        TotalPackageCount = container.TotalPackageCount,
            //        TotalGrossWeightKg = container.TotalGrossWeight,
            //        TotalNetWeightKg = container.TotalNetWeight,
            //        TareWeightKg = container.TareWeight,
            //        TotalVolumeCBM = container.TotalVolumeCBM,

            //        ExportPermissionNo = container.ExportPermissionNo,

            //        OverLength = container.OverLength,
            //        OverWidth = container.OverWidth,
            //        OverHeight = container.OverHeight,
            //        DimensionUnit = container.DimensionUnit?.MeasurementUnitName,

            //        Cargoes = container.CargoAllocations
            //            .OrderBy(x => x.CargoDetail.Sequence)
            //            .Select((allocation, cargoIndex) =>
            //                MapContainerCargo(allocation, cargoIndex + 1))
            //            .ToList()
            //    })
            //    .ToList();

            //var unallocatedCargoes = bl.CargoDetails
            //    .Where(x => !containerIdsWithAllocatedCargo.Contains(x.Id))
            //    .OrderBy(x => x.Sequence)
            //    .Select(x => new BLManifestUnallocatedCargoVm
            //    {
            //        Sequence = x.Sequence,
            //        CargoDescription = x.CargoDescription,
            //        MarksAndNumbers = x.MarksAndNumbers,
            //        HSCode = x.HSCodeToSave,

            //        PackageCount = x.PackageCount,
            //        PackageType = x.PackageType?.PackageTypeName,

            //        GrossWeightKg = x.GrossWeight,
            //        NetWeightKg = x.NetWeight,
            //        VolumeCBM = x.Volume,
            //        Remarks = x.Remarks
            //    })
            //    .ToList();


            var activeContainers = bl.BLContainers
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.ContainerNumber)
                .ToList();

            var allocatedCargoDetailIds = activeContainers
                .SelectMany(x => x.CargoAllocations)
                .Where(x => !x.IsDeleted)
                .Select(x => x.CargoDetailId)
                .Distinct()
                .ToHashSet();

            var containers = activeContainers
                .Select((container, containerIndex) => new BLManifestContainerVm
                {
                    RowNo = containerIndex + 1,

                    ContainerNumber = container.ContainerNumber,

                    ContainerSizeType = container.ContainerSizeType?.ShortName,
                    LoadType = container.LoadType?.LoadTypeCode,
                    FullMT = container.FullMT?.FullMTName,
                    OwnershipType = container.CntOwnType?.CntOwnTypeName,

                    SealNumber = container.SealNumber,
                    SealNumber2 = container.SealNumber2,

                    TotalPackageCount = container.TotalPackageCount,
                    TotalGrossWeightKg = container.TotalGrossWeight,
                    TotalNetWeightKg = container.TotalNetWeight,
                    TareWeightKg = container.TareWeight,
                    TotalVolumeCBM = container.TotalVolumeCBM,

                    ExportPermissionNo = container.ExportPermissionNo,

                    OverLength = container.OverLength,
                    OverWidth = container.OverWidth,
                    OverHeight = container.OverHeight,
                    DimensionUnit = container.DimensionUnit?.MeasurementUnitName,

                    Cargoes = container.CargoAllocations
                        .Where(x => !x.IsDeleted)
                        .Where(x => x.CargoDetail != null)
                        .OrderBy(x => x.CargoDetail.Sequence)
                        .Select((allocation, cargoIndex) =>
                            MapContainerCargo(allocation, cargoIndex + 1))
                        .ToList()
                })
                .ToList();

            var unallocatedCargoes = bl.CargoDetails
                .Where(x => !x.IsDeleted)
                .Where(x => !allocatedCargoDetailIds.Contains(x.Id))
                .OrderBy(x => x.Sequence)
                .Select(x => new BLManifestUnallocatedCargoVm
                {
                    Sequence = x.Sequence,
                    CargoDescription = x.CargoDescription,
                    MarksAndNumbers = x.MarksAndNumbers,
                    HSCode = x.HSCodeToSave,

                    PackageCount = x.PackageCount,
                    PackageType = x.PackageType?.PackageTypeName,

                    GrossWeightKg = x.GrossWeight,
                    NetWeightKg = x.NetWeight,
                    VolumeCBM = x.Volume,
                    Remarks = x.Remarks
                })
                .ToList();

            var cargoRows = BuildManifestCargoRows(containers);




            return new BLManifestReportVm
            {
                BLId = bl.Id,

                // Document
                BLNo = bl.BLNo,
                MasterBLNo = bl.MasterBLNo,
                BLType = bl.BLType?.BLTypeName,
                BLLevel = bl.BLLevel?.BLLevelName,
                BLIssueDate = bl.BLIssueDate,
                PlaceOfIssue = bl.PlaceOfIssue,

                // این شماره می‌تواند بعداً طبق الگوی شما تولید شود.
                ManifestReference = $"MAN-{bl.BLNo}",
                PrintedAt = DateTime.Now,

                // Carrier / Issuer
                CarrierName = bl.Carrier?.Person.PersonName,
                IssuerName = ResolveIssuerName(bl),
                IssuedByUs = bl.IssuedByUs,

                // Commercial
                FreightTerms = bl.FreightTerms,
                PaymentType = bl.PaymentType?.PaymentTypeName,
                LoadType = bl.LoadType?.LoadTypeCode,
                MovementType = bl.MovementType?.MovementTypeCode,
                Incoterm = bl.Incoterm?.IncotermCode,
                ExportReference = bl.ExportReference,
                ForwardingAgentReference = bl.ForwardingAgentReference,
                MRN = bl.MRN,
                PursuitCode = bl.PursuitCode,

                // Routing
                POR = bl.PORText,
                POL = bl.POLText,
                POD = bl.PODText,
                PODel = bl.PODelText,
                FinalDestination = bl.FinalDestinationText,
                Origin = bl.PointandCountryOfOriginText,
                OnwardInlandRouting = bl.OnwardInlandRoutingText,

                // اگر Voyage Main Carriage مقدار داشته باشد،
                // در View از آن استفاده خواهیم کرد.
                ReceivedForShipmentDate = bl.ReceivedForShipmentDate,
                ShippedOnBoardDate = bl.ShippedOnBoardDate,

                // Parties
                Shipper = MapParty("Shipper", shipper),
                Consignee = MapParty("Consignee", consignee),
                NotifyParty = MapNotifyParty(notifyParty, consignee),

                // Voyages
                Voyages = bl.BLVoyages
                    .OrderBy(x => x.VoyageTypeId)
                    .Select(x => new BLManifestVoyageVm
                    {
                        VoyageType = x.VoyageType?.VoyageTypeName,
                        VesselName = x.VesselName,
                        VoyageNo = x.VoyageNo,
                        ShippedOnBoardDate = x.ShippedOnBoardDate,
                        ReceivedForShipmentDate = x.ReceivedForShipmentDate,

                        // مقدار واقعی MainCarriage را مطابق Lookup خودتان تنظیم کنید.
                        IsMainCarriage = false
                    })
                    .ToList(),

                Containers = containers,
                UnallocatedCargoes = unallocatedCargoes,
                CargoRows = cargoRows,


                // Totals ثبت‌شده روی B/L
                DeclaredTotalContainers = bl.TotalContainers,
                DeclaredTotalPackages = bl.TotalPackages,
                DeclaredGrossWeightKg = bl.GrossWeightKg,
                DeclaredNetWeightKg = bl.NetWeightKg,
                DeclaredVolumeCBM = bl.VolumeCBM,

                // Totals محاسبه‌شده از کانتینرها
                //CalculatedTotalContainers = bl.BLContainers.Count,
                //CalculatedGrossWeightKg = bl.BLContainers.Sum(x => x.TotalGrossWeight ?? 0m),
                //CalculatedNetWeightKg = bl.BLContainers.Sum(x => x.TotalNetWeight ?? 0m),
                //CalculatedVolumeCBM = bl.BLContainers.Sum(x => x.TotalVolumeCBM ?? 0m),

                CalculatedTotalContainers = activeContainers.Count,
                CalculatedGrossWeightKg = activeContainers.Sum(x => x.TotalGrossWeight ?? 0m),
                CalculatedNetWeightKg = activeContainers.Sum(x => x.TotalNetWeight ?? 0m),
                CalculatedVolumeCBM = activeContainers.Sum(x => x.TotalVolumeCBM ?? 0m),

                GoodsDescription = bl.GoodsDescription,
                MarksAndNumbers = bl.MarksAndNumbers,
                RemarksInManifest = bl.RemarksInManifest,
                ClauseRemarks = bl.ClauseRemarks,
                OnBoardNotation = bl.OnBoardNotation,
                CalculatedTotalPackages = cargoRows
                                    .Where(x => x.PackageCount.HasValue)
                                    .Sum(x => (int)x.PackageCount!.Value),
            };
        }




        public async Task<BLDocumentReportVm?> GetBillOfLadingAsync(
            Guid blId,
            bool isOriginal,
            CancellationToken cancellationToken = default)
        {
            var bl = await _dbContext.BLs
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.Id == blId && !x.IsDeleted)

                .Include(x => x.Company)
                .Include(x => x.Carrier)
                    .ThenInclude(x => x.Person)
                .Include(x => x.IssuerParty)

                .Include(x => x.PaymentType)
                .Include(x => x.LoadType)
                .Include(x => x.MovementType)
                .Include(x => x.Incoterm)
                .Include(x => x.BLType)
                .Include(x => x.BLLevel)
                .Include(x => x.BLReleaseType)
                .Include(x => x.PackageType)
                .Include(x => x.FreightPaymentAt)

                .Include(x => x.BLParties)
                    .ThenInclude(x => x.PartyRoleType)

                .Include(x => x.BLVoyages)
                    .ThenInclude(x => x.VoyageType)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.ContainerSizeType)
                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CntOwnType)
                .FirstOrDefaultAsync(cancellationToken);

            if (bl is null)
            {
                return null;
            }

            /*
             * قانون کسب‌وکار پایه:
             * B/L کنسل‌شده (StatusId = 99) نباید به‌صورت Original چاپ شود.
             *
             * برای Draft می‌توانید اجازه نمایش بدهید یا در صورت سیاست شرکت،
             * آن را نیز ممنوع کنید.
             */
            if (isOriginal && bl.BLStatusId == 99)
            {
                throw new InvalidOperationException(
                    "A cancelled Bill of Lading cannot be printed as Original.");
            }

            var shipper = bl.BLParties
                .FirstOrDefault(x => x.RoleTypeId == BLPartyRoleTypeIds.Shipper);

            var consignee = bl.BLParties
                .FirstOrDefault(x => x.RoleTypeId == BLPartyRoleTypeIds.Consignee);

            var notifyParty = bl.BLParties
                .FirstOrDefault(x => x.RoleTypeId == BLPartyRoleTypeIds.NotifyParty);

            /*
             * فعلاً Voyage اصلی را از اولین رکورد Voyage انتخاب می‌کنیم.
             *
             * در گام بعد بهتر است مقدار واقعی VoyageTypeId مربوط به
             * Main Carriage را در یک Constant یا Lookup مشخص کنید.
             */
            var mainVoyage = bl.BLVoyages
                .OrderBy(x => x.VoyageTypeId)
                .FirstOrDefault();

            var preCarriageVoyage = bl.BLVoyages
                .OrderBy(x => x.VoyageTypeId)
                .Skip(1)
                .FirstOrDefault();

            var containers = bl.BLContainers
                .OrderBy(x => x.ContainerNumber)
                .Select((container, index) => new BLDocumentContainerVm
                {
                    RowNo = index + 1,
                    ContainerNumber = container.ContainerNumber,

                    // اگر property نمایشی شما Name نیست، آن را اصلاح کنید.
                    ContainerSizeType = container.ContainerSizeType?.ShortName,

                    SealNumber = container.SealNumber,
                    SealNumber2 = container.SealNumber2,

                    TotalPackageCount = container.TotalPackageCount,
                    TotalGrossWeightKg = container.TotalGrossWeight,
                    TotalVolumeCBM = container.TotalVolumeCBM,
                    OwnType = container.CntOwnType.CntOwnTypeName,
                    MovementType = bl.LoadType.LoadTypeCode,
                    PackageType = bl.PackageType.PackageTypeName,
                    TareWeight = container.TareWeight is null ? 0 : container.TareWeight.Value
                })
                .ToList();

            return new BLDocumentReportVm
            {
                BLId = bl.Id,

                // وضعیت سند
                IsDraft = !isOriginal,
                IsOriginal = isOriginal,
                CompanyId = bl.CompanyId,

                /*
                 * بهتر است مقدار IsNegotiable را طبق BLType یا BLLevel واقعی
                 * سیستم خودتان تعیین کنید. در این مرحله مقدار پیش‌فرض گذاشته شده.
                 */
                IsNegotiable = isOriginal,
                DocumentCaption = "BILL OF LADING",

                // Issuer / Carrier
                IssuerName = ResolveIssuerName(bl),
                IssuerAddress = ResolveIssuerAddress(bl),
                IssuerTelephone = null,
                IssuerEmail = null,
                IssuerWebsite = null,
                CarrierName = bl.Carrier?.Person.PersonName,

                // B/L Document
                BLNo = bl.BLNo,
                MasterBLNo = bl.MasterBLNo,
                BLType = bl.BLType?.BLTypeName,
                BLLevel = bl.BLLevel?.BLLevelName,
                BLIssueDate = bl.BLIssueDate,
                PlaceOfIssue = bl.PlaceOfIssue,

                OriginalCount = bl.OriginalCount,
                CopyCount = bl.CopyCount,

                ReleaseType = bl.BLReleaseType?.BLReleaseTypeName,
                ReleaseDate = bl.ReleaseDate,

                // Parties
                Shipper = MapParty("Shipper", shipper),
                Consignee = MapParty("Consignee", consignee),
                NotifyParty = MapNotifyParty(notifyParty, consignee),

                // Routing
                PlaceOfReceipt = bl.PORText,
                PortOfLoading = bl.POLText,
                PortOfDischarge = bl.PODText,
                PlaceOfDelivery = bl.PODelText,
                FinalDestination = bl.FinalDestinationText,

                PointAndCountryOfOrigin = bl.PointandCountryOfOriginText,
                OnwardInlandRouting = bl.OnwardInlandRoutingText,

                // Sea Voyage
                PreCarriageBy = GetVoyageDisplayText(preCarriageVoyage),
                MainVesselName = mainVoyage?.VesselName,
                MainVoyageNo = mainVoyage?.VoyageNo,

                // اولویت با تاریخ‌های Voyage اصلی است؛ اگر نبود تاریخ خود B/L
                ReceivedForShipmentDate =
                    mainVoyage?.ReceivedForShipmentDate ??
                    bl.ReceivedForShipmentDate,

                ShippedOnBoardDate =
                    mainVoyage?.ShippedOnBoardDate ??
                    bl.ShippedOnBoardDate,

                Voyages = bl.BLVoyages
                    .OrderBy(x => x.VoyageTypeId)
                    .Select(x => new BLManifestVoyageVm
                    {
                        VoyageType = x.VoyageType?.VoyageTypeName,
                        VesselName = x.VesselName,
                        VoyageNo = x.VoyageNo,
                        ReceivedForShipmentDate = x.ReceivedForShipmentDate,
                        ShippedOnBoardDate = x.ShippedOnBoardDate,

                        // بعداً براساس MainCarriageId صحیح می‌شود.
                        IsMainCarriage = x.Id == mainVoyage?.Id
                    })
                    .ToList(),

                // Commercial
                FreightTerms = bl.FreightTerms,
                PaymentType = bl.PaymentType?.PaymentTypeName,
                FreightPaymentAt = bl.FreightPaymentAtText
                                    ?? bl.FreightPaymentAt?.LocationName,
                Incoterm = bl.Incoterm?.IncotermCode,
                ExportReference = bl.ExportReference,
                ForwardingAgentReference = bl.ForwardingAgentReference,
                LCNo = bl.LCNo,
                LCBankName = bl.LCBankName,

                // Cargo
                MarksAndNumbers = bl.MarksAndNumbers,
                GoodsDescription = bl.GoodsDescription,

                TotalPackages = bl.TotalPackages,
                PackageType = bl.PackageTypeText ?? bl.PackageType?.PackageTypeName,
                GrossWeightKg = bl.GrossWeightKg,
                NetWeightKg = bl.NetWeightKg,
                VolumeCBM = bl.VolumeCBM,

                Containers = containers,

                // Clauses / Remarks
                ClauseRemarks = bl.ClauseRemarks,
                OnBoardNotation = bl.OnBoardNotation,
                RemarksInBL = bl.RemarksInBL,

                /*
                 * در مرحله فعلی می‌توان نام Issuer را برای امضا نمایش داد.
                 * بعداً بهتر است یک مدل Signature/AuthorizedSignatory مستقل داشته باشید.
                 */
                SignedByName = ResolveIssuerName(bl),
                SignedByTitle = "As Agent for the Carrier",

                PrintedAt = DateTime.Now
            };
        }

        private static BLManifestContainerCargoVm MapContainerCargo(
            BLContainerCargoAllocation allocation,
            int rowNo)
        {
            var cargo = allocation.CargoDetail;

            return new BLManifestContainerCargoVm
            {
                RowNo = rowNo,

                CargoDescription = cargo.CargoDescription,
                MarksAndNumbers = cargo.MarksAndNumbers,
                HSCode = cargo.HSCodeToSave,

                PackageCount = allocation.PackageCount,
                PackageType = cargo.PackageType?.PackageTypeName,

                GrossWeightKg = allocation.GrossWeight,
                NetWeightKg = allocation.NetWeight,
                VolumeCBM = allocation.Volume,

                IsDG = cargo.IsDG,
                DGClass = cargo.DGClass?.DGClassName,
                UNNumber = cargo.UNNumber,
                PackingGroup = cargo.DGPackingGroup?.DGPackingGroupCode,
                ProperShippingName = cargo.PSN,
                MarinePollutant = cargo.MarinePolutant,

                IsTemperatureControlled = cargo.IsTemperatureControlled,
                TemperatureRequirement = BuildTemperatureRequirement(cargo),

                IsOverSized = cargo.IsOverSized,
                Dimensions = BuildDimensions(cargo),

                IsPerishable = cargo.IsPerishable,
                IsFragile = cargo.IsFragile,
                Remarks = allocation.Remarks ?? cargo.Remarks
            };
        }

        private static BLManifestPartyVm? MapParty(string title, BLParty? party)
        {
            if (party is null)
            {
                return null;
            }

            return new BLManifestPartyVm
            {
                Title = title,
                DisplayText = party.PartyTextInBL,
                Telephone = party.PartyTelephone,
                Mobile = party.PartyMobile,
                Fax = party.PartyFax,
                Email = party.PartyEmail,

                IsToOrder = party.IsConsigneeToOrder,
                IsToOrderOf = party.IsConsigneeToOrderOf,
                IsToBearer = party.IsConsigneeToBearer,
                IsSameAsConsignee = party.IsNotifySameAs
            };
        }

        private static BLManifestPartyVm? MapNotifyParty(
            BLParty? notifyParty,
            BLParty? consignee)
        {
            if (notifyParty is null)
            {
                return null;
            }

            if (notifyParty.IsNotifySameAs)
            {
                return new BLManifestPartyVm
                {
                    Title = "Notify Party",
                    DisplayText = "SAME AS CONSIGNEE",
                    IsSameAsConsignee = true
                };
            }

            return MapParty("Notify Party", notifyParty);
        }

        private static string? ResolveIssuerName(BL bl)
        {
            if (bl.IssuedByUs)
            {
                // بر اساس property واقعی Company خودتان اصلاح شود.
                return bl.Company?.Person.PersonName;
            }

            // بر اساس property واقعی Person خودتان اصلاح شود.
            return bl.IssuerParty?.PersonName;
        }

        private static string? BuildTemperatureRequirement(CargoDetail cargo)
        {
            if (!cargo.IsTemperatureControlled)
            {
                return null;
            }

            // Property Name را مطابق MeasurementUnit خودتان اصلاح کنید.
            var unit = cargo.TempUnit?.MeasurementUnitName ?? "°C";

            if (cargo.MinTemp.HasValue &&
                cargo.MaxTemp.HasValue)
            {
                return $"{cargo.MinTemp:0.##} to {cargo.MaxTemp:0.##} {unit}";
            }

            if (cargo.NormalTemp.HasValue)
            {
                return $"{cargo.NormalTemp:0.##} {unit}";
            }

            return "Temperature Controlled";
        }

        private static string? BuildDimensions(CargoDetail cargo)
        {
            if (!cargo.IsOverSized)
            {
                return null;
            }

            if (!cargo.Length.HasValue &&
                !cargo.Width.HasValue &&
                !cargo.Height.HasValue)
            {
                return "OOG";
            }

            // Property Name را مطابق MeasurementUnit خودتان اصلاح کنید.
            var unit = cargo.DimensionUnit?.MeasurementUnitName ?? string.Empty;

            return $"{cargo.Length:0.##} × {cargo.Width:0.##} × {cargo.Height:0.##} {unit}".Trim();
        }


        private static string? GetVoyageDisplayText(BLVoyage? voyage)
        {
            if (voyage is null)
            {
                return null;
            }

            var vesselName = voyage.VesselName?.Trim();
            var voyageNo = voyage.VoyageNo?.Trim();

            if (string.IsNullOrWhiteSpace(vesselName))
            {
                return voyageNo;
            }

            if (string.IsNullOrWhiteSpace(voyageNo))
            {
                return vesselName;
            }

            return $"{vesselName} / {voyageNo}";
        }

        private static string? ResolveIssuerAddress(BL bl)
        {
            /*
             * هنوز در کلاس BL اطلاعات Address مربوط به Issuer/Company
             * به‌صورت مستقیم قابل دسترسی نیست؛ بنابراین فعلاً null برمی‌گردانیم.
             *
             * پس از مشخص شدن ساختار Company / Branch / Address،
             * این متد باید از آن اطلاعات مقداردهی شود.
             */
            return null;
        }




        public async Task<ContainerPackingListReportVm?> GetContainerPackingListAsync(
            Guid blId,
            CancellationToken cancellationToken = default)
        {
            var bl = await _dbContext.BLs
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.Id == blId && !x.IsDeleted)

                .Include(x => x.Company)
                    .ThenInclude(x => x.Person)
                .Include(x => x.Carrier)
                    .ThenInclude(x => x.Person)

                .Include(x => x.BLParties)
                    .ThenInclude(x => x.PartyRoleType)

                .Include(x => x.BLVoyages)
                    .ThenInclude(x => x.VoyageType)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.ContainerSizeType)
                        .ThenInclude(x => x.ContainerType)

                // نام واقعی این Navigation را با مدل خودتان تطبیق دهید.
                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CargoAllocations)
                        .ThenInclude(x => x.CargoDetail)
                            .ThenInclude(x => x.CargoCommodity)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CargoAllocations)
                        .ThenInclude(x => x.CargoDetail)
                            .ThenInclude(x => x.PackageType)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CargoAllocations)
                        .ThenInclude(x => x.CargoDetail)
                            .ThenInclude(x => x.DGClass)

                .FirstOrDefaultAsync(cancellationToken);

            if (bl is null)
            {
                return null;
            }

            var shipper = bl.BLParties
                .FirstOrDefault(x => x.RoleTypeId == BLPartyRoleTypeIds.Shipper);

            var consignee = bl.BLParties
                .FirstOrDefault(x => x.RoleTypeId == BLPartyRoleTypeIds.Consignee);

            var mainVoyage = bl.BLVoyages
                .FirstOrDefault(x => x.VoyageTypeId == 2)    //Main
                ?? bl.BLVoyages
                    .OrderBy(x => x.VoyageTypeId)
                    .FirstOrDefault();

            var containers = bl.BLContainers
                .OrderBy(x => x.ContainerNumber)
                .Select((container, containerIndex) =>
                {
                    var cargoItems = container.CargoAllocations
                        .Where(x => !x.IsDeleted)
                        .OrderBy(x => x.Id)
                        .ThenBy(x => x.CreatedAt)
                        .Select((allocation, cargoIndex) => new ContainerPackingListCargoVm
                        {
                            RowNo = cargoIndex + 1,

                            CommodityName = allocation.CargoDetail?.CargoCommodity?.CargoCommodityName,
                            CargoDescription = allocation.CargoDetail?.CargoDescription,
                            MarksAndNumbers = allocation.CargoDetail?.MarksAndNumbers,

                            PackageCount = (int)allocation.PackageCount,
                            PackageType = allocation.CargoDetail?.PackageType?.PackageTypeName,

                            GrossWeightKg = allocation.GrossWeight,
                            NetWeightKg = allocation.NetWeight,
                            VolumeCBM = allocation.Volume,

                            IsDangerousGoods = allocation.CargoDetail.IsDG,
                            UNNo = allocation.CargoDetail?.UNNumber,
                            IMOClass = allocation.CargoDetail?.DGClass?.DGClassName,
                            PackingGroup = allocation.CargoDetail?.DGPackingGroup?.DGPackingGroupCode,

                            Remarks = allocation.Remarks
                        })
                        .ToList();

                    return new ContainerPackingListContainerVm
                    {
                        RowNo = containerIndex + 1,

                        ContainerNumber = container.ContainerNumber,
                        ContainerSizeType = container.ContainerSizeType?.ShortName,
                        ISOCode = container.ContainerSizeType?.IsoCode,

                        SealNumber = container.SealNumber,
                        SealNumber2 = container.SealNumber2,

                        TareWeightKg = container.TareWeight,
                        MaxGrossWeightKg = container.TotalGrossWeight,
                        VgmWeightKg = container.TotalGrossWeight,

                        IsReefer = container.ContainerSizeType.IsReefer,
                        //TemperatureCelsius = container.,
                        //TemperatureUnit = container.TemperatureUnit,

                        HasDangerousGoods = cargoItems.Any(x => x.IsDangerousGoods),

                        TotalPackages = cargoItems.Sum(x => x.PackageCount),
                        TotalGrossWeightKg = cargoItems.Sum(x => x.GrossWeightKg),
                        TotalNetWeightKg = cargoItems.Sum(x => x.NetWeightKg),
                        TotalVolumeCBM = cargoItems.Sum(x => x.VolumeCBM),

                        //ContainerRemarks = container.Re,
                        CargoItems = cargoItems
                    };
                })
                .ToList();

            return new ContainerPackingListReportVm
            {
                BLId = bl.Id,

                BLNo = bl.BLNo,
                MasterBLNo = bl.MasterBLNo,

                CompanyName = bl.Company?.Person.PersonName,
                CarrierName = bl.Carrier?.Person.PersonName,

                ShipperText = shipper?.PartyTextInBL,
                ConsigneeText = consignee?.PartyTextInBL,

                PlaceOfReceipt = bl.PORText,
                PortOfLoading = bl.POLText,
                PortOfDischarge = bl.PODText,
                PlaceOfDelivery = bl.PODelText,

                VesselName = mainVoyage?.VesselName,
                VoyageNo = mainVoyage?.VoyageNo,

                IsCancelled = bl.BLStatusId == 99,
                PrintedAt = DateTime.Now,

                TotalContainers = containers.Count,
                TotalPackages = containers.Sum(x => x.TotalPackages),
                TotalGrossWeightKg = containers.Sum(x => x.TotalGrossWeightKg),
                TotalNetWeightKg = containers.Sum(x => x.TotalNetWeightKg),
                TotalVolumeCBM = containers.Sum(x => x.TotalVolumeCBM),

                Containers = containers
            };
        }

        public async Task<CargoAllocationByContainerReportVm?> GetCargoAllocationByContainerAsync(
            Guid blId,
            CancellationToken cancellationToken = default)
        {
            var bl = await _dbContext.BLs
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.Id == blId && !x.IsDeleted)

                .Include(x => x.Company)
                    .ThenInclude(x => x.Person)
                .Include(x => x.Carrier)
                    .ThenInclude(x => x.Person)
                .Include(x => x.BLParties)
                    .ThenInclude(x => x.PartyRoleType)

                .Include(x => x.BLVoyages)
                    .ThenInclude(x => x.VoyageType)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.ContainerSizeType)
                        .ThenInclude(x => x.ContainerType)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CargoAllocations)
                        .ThenInclude(x => x.CargoDetail)
                            .ThenInclude(x => x.CargoCommodity)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CargoAllocations)
                        .ThenInclude(x => x.CargoDetail)
                            .ThenInclude(x => x.PackageType)

                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.CargoAllocations)
                        .ThenInclude(x => x.CargoDetail)
                            .ThenInclude(x => x.DGClass)

                .FirstOrDefaultAsync(cancellationToken);

            if (bl is null)
            {
                return null;
            }

            var shipper = bl.BLParties
                .FirstOrDefault(x => x.RoleTypeId == BLPartyRoleTypeIds.Shipper);

            var consignee = bl.BLParties
                .FirstOrDefault(x => x.RoleTypeId == BLPartyRoleTypeIds.Consignee);

            var mainVoyage = bl.BLVoyages
                .FirstOrDefault(x => x.VoyageTypeId == 2)
                ?? bl.BLVoyages
                    .OrderBy(x => x.VoyageTypeId)
                    .FirstOrDefault();

            var rows = bl.BLContainers
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.ContainerNumber)
                .SelectMany(container => container.CargoAllocations
                    .Where(allocation => !allocation.IsDeleted)
                    //.OrderBy(allocation => allocation.SortOrder)
                    .OrderBy(allocation => allocation.CreatedAt)
                    .Select(allocation => new
                    {
                        Container = container,
                        Allocation = allocation
                    }))
                .Select((item, index) => new CargoAllocationByContainerRowVm
                {
                    RowNo = index + 1,

                    ContainerNumber = item.Container.ContainerNumber,
                    ContainerSizeType = item.Container.ContainerSizeType?.ShortName,
                    ISOCode = item.Container.ContainerSizeType?.IsoCode,

                    SealNumber = item.Container.SealNumber,
                    SealNumber2 = item.Container.SealNumber2,

                    IsReefer = item.Container.ContainerSizeType.IsReefer,
                    //TemperatureCelsius = item.Container.CargoAllocations,

                    CommodityName = item.Allocation.CargoDetail.CargoCommodity?.CargoCommodityName,
                    CargoDescription = item.Allocation.CargoDetail.CargoDescription,
                    MarksAndNumbers = item.Allocation.CargoDetail.MarksAndNumbers,

                    PackageCount = (int)item.Allocation.PackageCount,
                    PackageType = item.Allocation.CargoDetail.PackageType?.PackageTypeName,

                    GrossWeightKg = item.Allocation.GrossWeight,
                    NetWeightKg = item.Allocation.NetWeight,
                    VolumeCBM = item.Allocation.Volume,

                    IsDangerousGoods = item.Allocation.CargoDetail.IsDG,
                    UNNo = item.Allocation.CargoDetail.UNNumber,
                    IMOClass = item.Allocation.CargoDetail.DGClass.DGClassNo,
                    PackingGroup = item.Allocation.CargoDetail.DGPackingGroup.DGPackingGroupCode,

                    Remarks = item.Allocation.Remarks
                })
                .ToList();

            return new CargoAllocationByContainerReportVm
            {
                BLId = bl.Id,

                BLNo = bl.BLNo,
                MasterBLNo = bl.MasterBLNo,

                CompanyName = bl.Company?.Person.PersonName,
                CarrierName = bl.Carrier?.Person.PersonName,

                ShipperText = shipper?.PartyTextInBL,
                ConsigneeText = consignee?.PartyTextInBL,

                VesselName = mainVoyage?.VesselName,
                VoyageNo = mainVoyage?.VoyageNo,

                PortOfLoading = bl.POLText,
                PortOfDischarge = bl.PODText,

                IsCancelled = bl.BLStatusId == 99,

                TotalContainers = bl.BLContainers.Count(x => !x.IsDeleted),
                TotalAllocationRows = rows.Count,
                TotalPackages = rows.Sum(x => x.PackageCount),

                TotalGrossWeightKg = rows.Sum(x => x.GrossWeightKg),
                TotalNetWeightKg = rows.Sum(x => x.NetWeightKg),
                TotalVolumeCBM = rows.Sum(x => x.VolumeCBM),

                PrintedAt = DateTime.Now,
                Rows = rows
            };
        }


        public async Task<Report> CreateContainerCargoManifestAsync(
            BLManifestReportVm model,
            CancellationToken cancellationToken = default)
        {
            var templatePath = Path.Combine(
                _environment.ContentRootPath,
                "Reports",
                "BL",
                "ContainerCargoManifest.frx");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException(
                    "Container Cargo Manifest template was not found.",
                    templatePath);
            }

            var report = new Report();

            report.Load(templatePath);

            report.RegisterData(
                new List<BLManifestReportVm> { model },
                "Manifest");

            report.RegisterData(
                model.Voyages,
                "ManifestVoyages");

            report.RegisterData(
                model.Containers,
                "ManifestContainers");

            report.RegisterData(
                model.CargoRows,
                "ManifestCargoRows");

            report.RegisterData(
                model.UnallocatedCargoes,
                "ManifestUnallocatedCargoes");

            EnableDataSource(report, "Manifest");
            EnableDataSource(report, "ManifestVoyages");
            EnableDataSource(report, "ManifestContainers");
            EnableDataSource(report, "ManifestCargoRows");
            EnableDataSource(report, "ManifestUnallocatedCargoes");

            report.Prepare();

            return report;
        }

        private static void EnableDataSource(
            Report report,
            string dataSourceName)
        {
            var dataSource = report.GetDataSource(dataSourceName);

            if (dataSource is not null)
            {
                dataSource.Enabled = true;
            }
        }

        private static List<BLManifestCargoRowVm> BuildManifestCargoRows(
            IReadOnlyCollection<BLManifestContainerVm> containers)
        {
            var result = new List<BLManifestCargoRowVm>();
            var rowNo = 1;

            foreach (var container in containers.OrderBy(x => x.RowNo))
            {
                var sealDisplay = JoinNonEmpty(
                    container.SealNumber,
                    container.SealNumber2);

                foreach (var cargo in container.Cargoes.OrderBy(x => x.RowNo))
                {
                    result.Add(new BLManifestCargoRowVm
                    {
                        RowNo = rowNo++,

                        // Container
                        ContainerRowNo = container.RowNo,
                        ContainerNumber = container.ContainerNumber,
                        ContainerSizeType = container.ContainerSizeType,
                        ContainerLoadType = container.LoadType,
                        FullMT = container.FullMT,
                        OwnershipType = container.OwnershipType,

                        SealNumber = container.SealNumber,
                        SealNumber2 = container.SealNumber2,
                        SealDisplay = sealDisplay,

                        ContainerTareWeightKg = container.TareWeightKg,
                        ExportPermissionNo = container.ExportPermissionNo,

                        // Cargo
                        CargoDescription = cargo.CargoDescription,
                        MarksAndNumbers = cargo.MarksAndNumbers,
                        HSCode = cargo.HSCode,

                        PackageCount = cargo.PackageCount,
                        PackageType = cargo.PackageType,

                        GrossWeightKg = cargo.GrossWeightKg,
                        NetWeightKg = cargo.NetWeightKg,
                        VolumeCBM = cargo.VolumeCBM,

                        // DG
                        IsDG = cargo.IsDG,
                        DGClass = cargo.DGClass,
                        UNNumber = cargo.UNNumber,
                        PackingGroup = cargo.PackingGroup,
                        ProperShippingName = cargo.ProperShippingName,
                        MarinePollutant = cargo.MarinePollutant,
                        DGDisplay = BuildDGDisplay(cargo),

                        // Special cargo
                        IsTemperatureControlled = cargo.IsTemperatureControlled,
                        TemperatureRequirement = cargo.TemperatureRequirement,

                        IsOverSized = cargo.IsOverSized,
                        Dimensions = cargo.Dimensions,

                        IsPerishable = cargo.IsPerishable,
                        IsFragile = cargo.IsFragile,

                        SpecialHandlingDisplay = BuildSpecialHandlingDisplay(cargo),
                        Remarks = cargo.Remarks
                    });
                }
            }

            return result;
        }

        private static string? BuildDGDisplay(
            BLManifestContainerCargoVm cargo)
        {
            if (!cargo.IsDG)
            {
                return null;
            }

            var values = new List<string>();

            if (!string.IsNullOrWhiteSpace(cargo.UNNumber))
            {
                values.Add($"UN {cargo.UNNumber}");
            }

            if (!string.IsNullOrWhiteSpace(cargo.DGClass))
            {
                values.Add($"Class {cargo.DGClass}");
            }

            if (!string.IsNullOrWhiteSpace(cargo.PackingGroup))
            {
                values.Add($"PG {cargo.PackingGroup}");
            }

            if (!string.IsNullOrWhiteSpace(cargo.ProperShippingName))
            {
                values.Add(cargo.ProperShippingName);
            }

            if (!string.IsNullOrWhiteSpace(cargo.MarinePollutant))
            {
                values.Add($"Marine Pollutant: {cargo.MarinePollutant}");
            }

            return values.Count == 0
                ? "DG"
                : string.Join(" / ", values);
        }

        private static string? BuildSpecialHandlingDisplay(
            BLManifestContainerCargoVm cargo)
        {
            var values = new List<string>();

            if (cargo.IsTemperatureControlled)
            {
                values.Add(
                    string.IsNullOrWhiteSpace(cargo.TemperatureRequirement)
                        ? "Temperature Controlled"
                        : $"Temp: {cargo.TemperatureRequirement}");
            }

            if (cargo.IsOverSized)
            {
                values.Add(
                    string.IsNullOrWhiteSpace(cargo.Dimensions)
                        ? "OOG"
                        : $"OOG: {cargo.Dimensions}");
            }

            if (cargo.IsPerishable)
            {
                values.Add("Perishable");
            }

            if (cargo.IsFragile)
            {
                values.Add("Fragile");
            }

            return values.Count == 0
                ? null
                : string.Join(" / ", values);
        }


        private static string? JoinNonEmpty(params string?[] values)
        {
            var result = values
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .ToList();

            return result.Count == 0
                ? null
                : string.Join(" / ", result);
        }

        public async Task<byte[]?> GenerateSeaBillOfLadingPdfAsync(
            Guid blId,
            bool isOriginal,
            CancellationToken cancellationToken = default)
        {
            var document = await GetBillOfLadingAsync(
                blId,
                isOriginal,
                cancellationToken);

            if (document is null)
            {
                return null;
            }

            var dataSet = BuildSeaBillOfLadingDataSet(document);

            using var report = new Report();

            var templatePath = Path.Combine(
                _environment.ContentRootPath,
                "Reports",
                "BL",
                "SeaBillOfLading.frx");

            report.Load(templatePath);
            report.RegisterData(dataSet, "ShipRate");
            report.GetDataSource("BL")!.Enabled = true;
            report.GetDataSource("Voyages")!.Enabled = true;
            report.GetDataSource("Containers")!.Enabled = true;


            var logoPath = Path.Combine(
                   _environment.WebRootPath,
                   "logos",
                   $"logo-{document.CompanyId}.png");
            var logoObject = report.FindObject("CompanyLogo") as PictureObject;
            if (logoObject is not null &&
                System.IO.File.Exists(logoPath))
            {
                using var sourceImage = Image.FromFile(logoPath);
                logoObject.Image = new Bitmap(sourceImage);
            }
            report.Prepare();
            await using var output = new MemoryStream();
            using var export = new PDFSimpleExport
            {
                ShowProgress = false
            };
            report.Export(export, output);
            return output.ToArray();
        }
        private static DataSet BuildSeaBillOfLadingDataSet(
            BLDocumentReportVm document)
        {
            var dataSet = new DataSet("ShipRate");

            var blTable = CreateBlTable();
            var voyagesTable = CreateVoyagesTable();
            var containersTable = CreateContainersTable();

            blTable.Rows.Add(
                DbValue(document.DocumentCaption),
                DbValue(document.BLNo),
                DbValue(document.MasterBLNo),

                DbValue(document.IssuerName),
                DbValue(document.IssuerAddress),
                DbValue(document.IssuerTelephone),
                DbValue(document.IssuerEmail),

                DbValue(document.CarrierName),

                DbValue(document.Shipper?.DisplayText),

                DbValue(document.Consignee?.DisplayText),

                DbValue(document.NotifyParty?.DisplayText),

                DbValue(document.PlaceOfReceipt),
                DbValue(document.PortOfLoading),
                DbValue(document.PortOfDischarge),
                DbValue(document.PlaceOfDelivery),
                DbValue(document.FinalDestination),

                DbValue(document.PointAndCountryOfOrigin),
                DbValue(document.OnwardInlandRouting),

                DbValue(document.PreCarriageBy),
                DbValue(document.MainVesselName),
                DbValue(document.MainVoyageNo),
                DbValue(document.ReceivedForShipmentDate),
                DbValue(document.ShippedOnBoardDate),

                DbValue(document.FreightTerms),
                DbValue(document.PaymentType),
                DbValue(document.FreightPaymentAt),
                DbValue(document.Incoterm),
                DbValue(document.ExportReference),
                DbValue(document.ForwardingAgentReference),
                DbValue(document.LCNo),
                DbValue(document.LCBankName),

                DbValue(document.MarksAndNumbers),
                DbValue(document.GoodsDescription),
                document.TotalPackages,
                DbValue(document.PackageType),
                DbValue(document.GrossWeightKg),
                DbValue(document.NetWeightKg),
                DbValue(document.VolumeCBM),

                DbValue(document.ClauseRemarks),
                DbValue(document.OnBoardNotation),
                DbValue(document.RemarksInBL),

                DbValue(document.PlaceOfIssue),
                DbValue(document.BLIssueDate),

                DbValue(document.SignedByName),
                DbValue(document.SignedByTitle)
            );

            foreach (var voyage in document.Voyages)
            {
                voyagesTable.Rows.Add(
                    DbValue(voyage.VoyageType),
                    DbValue(voyage.VesselName),
                    DbValue(voyage.VoyageNo),
                    DbValue(voyage.ReceivedForShipmentDate),
                    DbValue(voyage.ShippedOnBoardDate),
                    voyage.IsMainCarriage
                );
            }

            foreach (var container in document.Containers)
            {
                containersTable.Rows.Add(
                    container.RowNo,
                    DbValue(container.ContainerNumber),
                    DbValue(container.ContainerSizeType),
                    DbValue(container.SealNumber),
                    DbValue(container.SealNumber2),
                    container.TotalPackageCount,
                    DbValue(container.TotalGrossWeightKg),
                    DbValue(container.TotalVolumeCBM),
                    DbValue(container.OwnType),
                    DbValue(container.MovementType),
                    DbValue(container.TareWeight),
                    DbValue(container.PackageType)
                );
            }

            dataSet.Tables.Add(blTable);
            dataSet.Tables.Add(voyagesTable);
            dataSet.Tables.Add(containersTable);

            return dataSet;
        }

        private static DataTable CreateBlTable()
        {
            var table = new DataTable("BL");

            AddStringColumn(table, "DocumentCaption");
            AddStringColumn(table, "BLNo");
            AddStringColumn(table, "MasterBLNo");

            AddStringColumn(table, "IssuerName");
            AddStringColumn(table, "IssuerAddress");
            AddStringColumn(table, "IssuerTelephone");
            AddStringColumn(table, "IssuerEmail");

            AddStringColumn(table, "CarrierName");

            AddStringColumn(table, "ShipperName");

            AddStringColumn(table, "ConsigneeName");

            AddStringColumn(table, "NotifyPartyName");

            AddStringColumn(table, "PlaceOfReceipt");
            AddStringColumn(table, "PortOfLoading");
            AddStringColumn(table, "PortOfDischarge");
            AddStringColumn(table, "PlaceOfDelivery");
            AddStringColumn(table, "FinalDestination");

            AddStringColumn(table, "PointAndCountryOfOrigin");
            AddStringColumn(table, "OnwardInlandRouting");

            AddStringColumn(table, "PreCarriageBy");
            AddStringColumn(table, "MainVesselName");
            AddStringColumn(table, "MainVoyageNo");
            AddDateTimeColumn(table, "ReceivedForShipmentDate");
            AddDateTimeColumn(table, "ShippedOnBoardDate");

            AddStringColumn(table, "FreightTerms");
            AddStringColumn(table, "PaymentType");
            AddStringColumn(table, "FreightPaymentAt");
            AddStringColumn(table, "Incoterm");
            AddStringColumn(table, "ExportReference");
            AddStringColumn(table, "ForwardingAgentReference");
            AddStringColumn(table, "LCNo");
            AddStringColumn(table, "LCBankName");

            AddStringColumn(table, "MarksAndNumbers");
            AddStringColumn(table, "GoodsDescription");
            table.Columns.Add("TotalPackages", typeof(int));
            AddStringColumn(table, "PackageType");
            AddDecimalColumn(table, "GrossWeightKg");
            AddDecimalColumn(table, "NetWeightKg");
            AddDecimalColumn(table, "VolumeCBM");

            AddStringColumn(table, "ClauseRemarks");
            AddStringColumn(table, "OnBoardNotation");
            AddStringColumn(table, "RemarksInBL");

            AddStringColumn(table, "PlaceOfIssue");
            AddDateTimeColumn(table, "BLIssueDate");

            AddStringColumn(table, "SignedByName");
            AddStringColumn(table, "SignedByTitle");

            return table;
        }

        private static DataTable CreateVoyagesTable()
        {
            var table = new DataTable("Voyages");

            AddStringColumn(table, "VoyageType");
            AddStringColumn(table, "VesselName");
            AddStringColumn(table, "VoyageNo");
            AddDateTimeColumn(table, "ReceivedForShipmentDate");
            AddDateTimeColumn(table, "ShippedOnBoardDate");
            table.Columns.Add("IsMainCarriage", typeof(bool));

            return table;
        }

        private static DataTable CreateContainersTable()
        {
            var table = new DataTable("Containers");

            table.Columns.Add("RowNo", typeof(int));
            AddStringColumn(table, "ContainerNumber");
            AddStringColumn(table, "ContainerSizeType");
            AddStringColumn(table, "SealNumber");
            AddStringColumn(table, "SealNumber2");

            table.Columns.Add("TotalPackageCount", typeof(int));
            AddDecimalColumn(table, "TotalGrossWeightKg");
            AddDecimalColumn(table, "TotalVolumeCBM");
            table.Columns.Add("OwnType", typeof(string));
            table.Columns.Add("MovementType", typeof(string));
            AddDecimalColumn(table, "TareWeight");
            table.Columns.Add("PackageType", typeof(string));
            return table;
        }

        private static void AddStringColumn(
            DataTable table,
            string name)
        {
            table.Columns.Add(name, typeof(string));
        }

        private static void AddDateTimeColumn(
            DataTable table,
            string name)
        {
            table.Columns.Add(name, typeof(DateTime));
        }

        private static void AddDecimalColumn(
            DataTable table,
            string name)
        {
            table.Columns.Add(name, typeof(decimal));
        }

        private static object DbValue<T>(T? value)
        {
            return value is null
                ? DBNull.Value
                : value;
        }





    }
}
