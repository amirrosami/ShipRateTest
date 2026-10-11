using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class RateInquiryEmailService : IRateInquiryEmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRazorViewRenderService _razorViewRenderService;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;

        public RateInquiryEmailService(
            ApplicationDbContext context,
            IRazorViewRenderService razorViewRenderService,
            IEmailService emailService,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _razorViewRenderService = razorViewRenderService;
            _emailService = emailService;
            _currentUserService = currentUserService;
        }

        public async Task<string> BuildInquiryEmailHtmlAsync(Guid inquiryId, bool canSend, bool isPreview)
        {
            var vm = await BuildRateInquiryEmailVm(inquiryId, isPreview);
            vm.canSendEmail = canSend;
            var html = await _razorViewRenderService.RenderToStringAsync(
                "~/Views/Shared/EmailTemplates/_RateInquiryEmailTemplate.cshtml",
                vm
            );

            return html;
        }

        public async Task<string> BuildSellQuoteEmailHtmlAsync(Guid quoteId, bool canSend, bool isPreview)
        {
            var vm = await BuildSellQuoteEmailVm(quoteId, isPreview);
            vm.canSendEmail = canSend;
            var html = await _razorViewRenderService.RenderToStringAsync(
                "~/Views/Shared/EmailTemplates/_SellQuoteEmailTemplate.cshtml",
                vm
            );
            return html;
        }
        public async Task SendInquiryEmailAsync(Guid inquiryId)
        {
            var inquiry = await _context.RateRequestInquiries
                .Include(x => x.RateRequest)
                .Include(x => x.Vendor)
                .FirstOrDefaultAsync(x => x.Id == inquiryId);

            if (inquiry == null)
                throw new Exception("Inquiry not found.");

            if (string.IsNullOrWhiteSpace(inquiry.Vendor?.EmailAddress))
                throw new Exception("Vendor email is empty.");

            var html = await BuildInquiryEmailHtmlAsync(inquiryId, true, false);
            //await _emailService.SendEmailViaMailKitAsync("ShipRate@Sta-Transport.com", $"Rate Inquiry Request - {inquiry.InquiryNo}", html, true, "g.rostami1354@gmail.com", $"company-logo-{1}");

            await _emailService.SendEmailViaMailKitAsync(inquiry.Vendor.EmailAddress, $"Rate Inquiry Request - {inquiry.InquiryNo}", html, true, await _currentUserService.EmailAddress(), $"company-logo-{1}");

            inquiry.StatusId = 2;
            inquiry.SentDateTime = DateTime.Now;
            inquiry.SentByUserId = _currentUserService.UserId;







            await _context.SaveChangesAsync();
        }

        public async Task SendSellQuoteEmailAsync(Guid quoteId)
        {
            var sq = await _context.SellQuotes
                .Include(x => x.RateRequest)
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (sq == null)
                throw new Exception("Sell Quotation not found.");

            if (string.IsNullOrWhiteSpace(sq.RateRequest.EmailAddress))
                throw new Exception("Customer email is empty.");
            var html = await BuildSellQuoteEmailHtmlAsync(quoteId, true, false);

            await _emailService.SendEmailViaMailKitAsync("ShipRate@Sta-Transport.com", $"Sell Quotation Response - {sq.QuotationNo}", html, true, "g.rostami1354@gmail.com", $"company-logo-{1}");

            //await _emailService.SendEmailAsync(inquiry.Vendor.EmailAddress, $"Rate Inquiry Request - {inquiry.InquiryNo}", html, true);

            sq.StatusId = 2;
            sq.SentByUserDate = DateTime.Now;
            sq.SentByUserId = _currentUserService.UserId;
            await _context.SaveChangesAsync();
        }




        public async Task<int> SendAllDraftInquiryEmailsAsync(Guid rateRequestId)
        {
            var draftInquiryIds = await _context.RateRequestInquiries
                .Where(x => x.RateRequestId == rateRequestId && x.StatusId == 1)
                .Select(x => x.Id)
                .ToListAsync();
            int sentCount = 0;
            foreach (var inquiryId in draftInquiryIds)
            {
                await SendInquiryEmailAsync(inquiryId);
                sentCount++;
            }
            return sentCount;
        }


        private async Task<SellQuoteEmailVm> BuildSellQuoteEmailVm(Guid quoteId, bool isPreview)
        {
            var quote = await _context.SellQuotes
                .Include(x => x.Items)
                    .ThenInclude(x => x.SellCurrency)
                .Include(x => x.Items)
                    .ThenInclude(x => x.ChargeType)
                .Include(x => x.Items)
                    .ThenInclude(x => x.ContainerSizeType)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.Containers)
                        .ThenInclude(x => x.ContainerSizeType)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                        .ThenInclude(x => x.PackageType)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                        .ThenInclude(x => x.DGClass)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                        .ThenInclude(x => x.WeightUnit)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                        .ThenInclude(x => x.DimensionUnit)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                        .ThenInclude(x => x.VolumeUnit)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                        .ThenInclude(x => x.TempUnit)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.Origin)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.Destination)
                .Include(x => x.Carrier)
                .Include(x => x.CreatedByUser)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.Company)
                        .ThenInclude(x => x.Person)
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (quote == null)
                throw new Exception("Sell Quote not found.");

            var rateRequest = quote.RateRequest;
            var vm = new SellQuoteEmailVm
            {
                IsPreview = isPreview,
                CompanyId = quote.CompanyId,
                QuoteNo = quote.QuotationNo,
                QuoteDate = quote.QuotationDate,
                CustomerName = rateRequest.CustomerName,
                Origin = rateRequest.Origin.LocationName,
                Destination = rateRequest.Destination.LocationName,

                //TransportType = inquiry.TransportType.TransportTypeName,
                CargoReadyDate = rateRequest.CargoReadinessDate,
                Remarks = quote.Remarks,
                CompanyName = rateRequest.Company.Person.PersonName.ToUpper(),
                DepartmentName = "Pricing / Procurement Department",
                CompanyEmail = quote.CreatedByUser.Email,
                CompanyTel = quote.CreatedByUser.PhoneNumber,
                CompanyWebsite = "https://stashipping.com/",
                Containers = new List<SellQuoteContainerVm>(),
                CargoDetails = new List<CargoDetailDetailsVm>(),
                SellQuoteItems = new List<SellQuoteItemVm>(),
            };

            foreach (var item in quote.Items)
            {
                vm.SellQuoteItems.Add(
                    new SellQuoteItemVm
                    {
                        ChargeTypeName = item.ChargeType.ChargeTypeName,
                        Counts = item.SellQuantity,
                        LineNumber = item.LineNumber,
                        CurrencyCode = item.SellCurrency.CurrencyCode,
                        SizeTypeName = item.ContainerSizeType?.ShortName,
                        TotalAmount = item.SellTotalAmount,
                        UnitRate = item.SellUnitRate
                    });
            }
            foreach (var item in rateRequest.Containers)
            {
                vm.Containers.Add(
                    new SellQuoteContainerVm
                    {
                        Counts = (int)item.Counts,
                        SizeTypeName = item.ContainerSizeType.ShortName
                    });
            }
            foreach (var item in rateRequest.CargoDetails)
            {
                vm.CargoDetails.Add(
                    new CargoDetailDetailsVm
                    {
                        CargoDescription = item.CargoDescription,
                        DGClassName = item.DGClass?.DGClassName,
                        UNNumber = item.UNNumber,
                        DGFlashPoint = item.DGFlashPoint,
                        DimensionUnitName = item.DimensionUnit?.MeasurementUnitName,
                        WeightUnitName = item.WeightUnit?.MeasurementUnitName,
                        VolumeUnitName = item.VolumeUnit?.MeasurementUnitName,
                        GrossWeight = item.GrossWeight,
                        Height = item.Height,
                        IsDG = item.IsDG,
                        IsFragile = item.IsFragile,
                        IsOverSized = item.IsOverSized,
                        IsPerishable = item.IsPerishable,
                        IsRotateable = item.IsRotateable,
                        IsStackable = item.IsStackable,
                        IsTemperatureControlled = item.IsTemperatureControlled,
                        MinTemp = item.MinTemp,
                        MaxTemp = item.MaxTemp,
                        NormalTemp = item.NormalTemp,
                        PackageCount = item.PackageCount,
                        NetWeight = item.NetWeight,
                        PackageTypeName = item.PackageType?.PackageTypeName,
                        HSCode = item.HSCodeToSave,
                        Length = item.Length,
                        Remarks = item.Remarks,
                        Sequence = item.Sequence,
                        Volume = item.Volume,
                        Width = item.Width,
                        TempUnitName = item.TempUnit?.MeasurementUnitName,
                        MarksAndNumbers = item.MarksAndNumbers,
                        Id = item.Id,
                    });
            }
            return vm;
        }





        private async Task<RateInquiryEmailVm> BuildRateInquiryEmailVm(Guid inquiryId, bool isPreview)
        {
            var inquiry = await _context.RateRequestInquiries
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.Containers)
                        .ThenInclude(x => x.ContainerSizeType)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                        .ThenInclude(x => x.PackageType)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                        .ThenInclude(x => x.DGClass)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                        .ThenInclude(x => x.WeightUnit)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                        .ThenInclude(x => x.DimensionUnit)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                        .ThenInclude(x => x.VolumeUnit)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                        .ThenInclude(x => x.TempUnit)
                .Include(x => x.TransportType)
                .Include(x => x.Vendor)
                .Include(x => x.Origin)
                .Include(x => x.Destination)
                .Include(x => x.CreatedByUser)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.Company)
                        .ThenInclude(x => x.Person)
                .FirstOrDefaultAsync(x => x.Id == inquiryId);

            if (inquiry == null)
                throw new Exception("Inquiry not found.");

            var rateRequest = inquiry.RateRequest;

            var vm = new RateInquiryEmailVm
            {
                IsPreview = isPreview,
                InquiryNo = inquiry.InquiryNo,
                InquiryDate = inquiry.CreatedAt,
                ResponseDueDate = inquiry.ResponseDueDate,
                CompanyId = inquiry.RateRequest.CompanyId,
                VendorName = inquiry.Vendor?.PersonName,
                Origin = rateRequest.Origin.LocationName,
                Destination = rateRequest.Destination.LocationName,


                TransportType = inquiry.TransportType.TransportTypeName,
                //ServiceType = rateRequest.ServiceType,
                //CargoType = rateRequest.CargoType,
                //Incoterm = rateRequest.Incoterm,

                //GrossWeight = rateRequest.GrossWeight,
                //ChargeableWeight = rateRequest.ChargeableWeight,
                //Volume = rateRequest.Volume,

                //PackageCount = rateRequest.PackageCount,
                //PackageSummary = rateRequest.PackageSummary,

                CargoReadyDate = rateRequest.CargoReadinessDate,
                //Commodity = rateRequest.Commodity,
                Remarks = inquiry.RemarksInInquiry,
                CompanyName = rateRequest.Company.Person.PersonName.ToUpper(),
                DepartmentName = "Pricing / Procurement Department",
                CompanyEmail = rateRequest.CreatedByUser.Email,
                CompanyTel = rateRequest.CreatedByUser.PhoneNumber,
                CompanyWebsite = "https://stashipping.com/",
                Containers = new List<RateInquiryContainerVm>(),
                CargoDetails = new List<CargoDetailDetailsVm>()
            };

            //foreach (var item in rateRequest.Packages)
            //{
            //    vm.Packages.Add(
            //        new RateInquiryPackageVm
            //        {
            //            PackageType = item.PackageType.PackageTypeName,
            //            Height = item.TotalHeight,
            //            Length = item.TotalLength,
            //            Weight = item.TotalGrossWeight,
            //            Width = item.TotalWidth,
            //            Quantity = item.UnitCount,
            //            //WeightUnit = item.WeightMeasurementUnit.MeasurementUnitName,
            //            //DimensionUnit = item.LengthMeasurementUnit.MeasurementUnitName,
            //        }
            //        );
            //}

            foreach (var item in rateRequest.Containers)
            {
                vm.Containers.Add(
                    new RateInquiryContainerVm
                    {
                        Counts = (int)item.Counts,
                        SizeTypeName = item.ContainerSizeType.ShortName
                    });
            }
            foreach (var item in rateRequest.CargoDetails)
            {
                vm.CargoDetails.Add(
                    new CargoDetailDetailsVm
                    {
                        CargoDescription = item.CargoDescription,
                        DGClassName = item.DGClass?.DGClassName,
                        UNNumber = item.UNNumber,
                        DGFlashPoint = item.DGFlashPoint,
                        DimensionUnitName = item.DimensionUnit?.MeasurementUnitName,
                        WeightUnitName = item.WeightUnit?.MeasurementUnitName,
                        VolumeUnitName = item.VolumeUnit?.MeasurementUnitName,
                        GrossWeight = item.GrossWeight,
                        Height = item.Height,
                        IsDG = item.IsDG,
                        IsFragile = item.IsFragile,
                        IsOverSized = item.IsOverSized,
                        IsPerishable = item.IsPerishable,
                        IsRotateable = item.IsRotateable,
                        IsStackable = item.IsStackable,
                        IsTemperatureControlled = item.IsTemperatureControlled,
                        MinTemp = item.MinTemp,
                        MaxTemp = item.MaxTemp,
                        NormalTemp = item.NormalTemp,
                        PackageCount = item.PackageCount,
                        NetWeight = item.NetWeight,
                        PackageTypeName = item.PackageType?.PackageTypeName,
                        HSCode = item.HSCodeToSave,
                        Length = item.Length,
                        Remarks = item.Remarks,
                        Sequence = item.Sequence,
                        Volume = item.Volume,
                        Width = item.Width,
                        TempUnitName = item.TempUnit?.MeasurementUnitName,
                        MarksAndNumbers = item.MarksAndNumbers,
                        Id = item.Id,
                    });
            }
            return vm;
        }




        public async Task<bool> SendInquiryInAppAsync(Guid inquiryId)
        {
            var inquiry = await _context.RateRequestInquiries.FindAsync(inquiryId);
            if (inquiry == null) return false;
            inquiry.SentDateTime = DateTime.Now;
            inquiry.SentByUserId = _currentUserService.UserId;
            inquiry.StatusId = 2;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SendSellQuoteInAppAsync(Guid quoteId)
        {
            var quote = await _context.SellQuotes.FindAsync(quoteId);
            if (quote == null) return false;
            quote.QuotationDate = DateTime.Now;
            quote.SentByUserId = _currentUserService.UserId;
            quote.SentByUserDate = DateTime.Now;
            quote.StatusId = 2;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AcceptSellQuoteInAppAsync(Guid quoteId, string respondComments = "")
        {
            var quote = await _context.SellQuotes.FindAsync(quoteId);
            if (quote == null) return false;
            quote.CustomerRespondDate = DateTime.Now;
            quote.CustomerRespondUserId = _currentUserService.UserId;
            quote.CustomerRespondComments = respondComments;
            quote.StatusId = 3;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RejectSellQuoteInAppAsync(Guid quoteId, string respondComments = "")
        {
            var quote = await _context.SellQuotes.FindAsync(quoteId);
            if (quote == null) return false;
            quote.CustomerRespondDate = DateTime.Now;
            quote.CustomerRespondUserId = _currentUserService.UserId;
            quote.CustomerRespondComments = respondComments;
            quote.StatusId = 4;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}