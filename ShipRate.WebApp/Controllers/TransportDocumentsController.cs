using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs.BillOfLadings;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.Interfaces.Manifest;
using ShipRate.Application.Interfaces.Transit;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.BillOfLading;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    //[Route("jobs/{jobId:guid}/transport-documents")]
    public class TransportDocumentsController : Controller
    {
        private readonly ITransportDocumentService _transportDocumentService;
        private readonly IJobLookupService _lookupService;
        private readonly IVoyageManagementService _voyageService;
        private readonly ITripService _tripService;
        private readonly IFlightService _flightService;
        private readonly IBLManifestReportService _manifestReportService;
        private readonly IJobLookupService _jobLookupService;

        public TransportDocumentsController(ITransportDocumentService transportDocumentService, IJobLookupService lookupService, IVoyageManagementService voyageService, ITripService tripService, IFlightService flightService, IBLManifestReportService manifestReport, IJobLookupService jobLookupService)
        {
            _transportDocumentService = transportDocumentService;
            _lookupService = lookupService;
            _voyageService = voyageService;
            _tripService = tripService;
            _flightService = flightService;
            _manifestReportService = manifestReport;
            _jobLookupService = jobLookupService;
        }

        //[HttpGet("create")]
        [HttpGet]
        public async Task<IActionResult> Create(Guid jobId)
        {
            var model = await _transportDocumentService.BuildCreateModelAsync(jobId);
            return View("~/Views/TransportDocuments/Upsert.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid jobId, TransportDocumentUpsertVm model)
        {
            model.JobId = jobId;
            model.BLStatusId = -1;//Draft

            ValidateBLHeader(model);
            if (!ModelState.IsValid)
            {
                model = await RebuildInvalidCreateModelAsync(jobId, model);
                return View("Upsert", model);
            }

            var check = await _transportDocumentService.CheckAndInsertMasterFileAsync(model);
            if (!check.Success)
            {
                ModelState.AddModelError(string.Empty, check.Message);
                model = await RebuildInvalidCreateModelAsync(jobId, model);
                return View("Upsert", model);
            }
            var id = await _transportDocumentService.SaveAsync(jobId, model);
            TempData["SuccessMessage"] = "Transport document created successfully.";
            return RedirectToAction(nameof(Edit), new { jobId, id });
        }
        private void ValidateBLHeader(TransportDocumentUpsertVm model)
        {
            if (!model.TransportTypeId.HasValue)
                ModelState.AddModelError(nameof(model.TransportTypeId), "Transport Type is required.");

            if (!model.BLTypeId.HasValue)
                ModelState.AddModelError(nameof(model.BLTypeId), "BL Type is required.");

            if (!model.BLLevelId.HasValue)
                ModelState.AddModelError(nameof(model.BLLevelId), "BL Level is required.");

            //if (!model.BLStatusId.HasValue)
            //    ModelState.AddModelError(nameof(model.BLStatusId), "Status is required.");
        }
        [HttpGet]
        public async Task<IActionResult> List(Guid jobId)
        {
            var context = new BLContextDto
            {
                JobId = jobId,
            };
            var model = await _transportDocumentService.GetManagePageAsync(context);
            return PartialView("_BLList", model);
        }
        //[HttpGet("{id:guid}/edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid jobId, Guid id)
        {
            var model = await _transportDocumentService.BuildEditModelAsync(jobId, id);
            ViewData["ContainerSizeTypes"] = await _jobLookupService.GetContainerSizeTypesAsync();
            ViewData["LoadTypes"] = await _jobLookupService.GetLoadTypesAsync();
            ViewData["CntOwnTypes"] = await _jobLookupService.GetCntOwnTypesAsync();
            ViewData["FullMTs"] = await _jobLookupService.GetFullMTsAsync();
            return View("Upsert", model);
        }

        //[HttpPost("{id:guid}/edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid jobId, Guid id, TransportDocumentUpsertVm model)
        {
            if (model.Containers?.Count > 0)
            {
                var duplicateContainerNumbers = model.Containers
                    .Where(c => !string.IsNullOrWhiteSpace(c.ContainerNumber))
                    .GroupBy(c => c.ContainerNumber.Trim(), StringComparer.OrdinalIgnoreCase)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();
                if (duplicateContainerNumbers.Any())
                {
                    string duplicateList = string.Join(", ", duplicateContainerNumbers);

                    // کلید روی اصل کالکشن Containers
                    ModelState.AddModelError(
                        nameof(model.Containers),
                        $"Duplicate container number(s) found: {duplicateList}"
                    );
                    model.Id = id;
                    model.JobId = jobId;
                    return View("Upsert", model);
                }
            }

            model.Id = id;
            model.JobId = jobId;

            ValidateBLHeader(model);


            if (!ModelState.IsValid)
            {
                model = await RebuildInvalidEditModelAsync(jobId, id, model);
                return View("Upsert", model);
            }

            var check = await _transportDocumentService.CheckAndInsertMasterFileAsync(model);
            if (!check.Success)
            {
                ModelState.AddModelError(string.Empty, check.Message);
                model = await RebuildInvalidEditModelAsync(jobId, id, model);
                return View("Upsert", model);
            }
            await _transportDocumentService.SaveAsync(jobId, model);
            TempData["SuccessMessage"] = "BL updated successfully.";
            return RedirectToAction(nameof(Edit), new { jobId, id });
        }
        //[HttpPost("{id:guid}/delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Delete(Guid jobId, Guid id)
        //{
        //    await _transportDocumentService.DeleteAsync(jobId, id);

        //    TempData["SuccessMessage"] = "Transport document deleted successfully.";

        //    return RedirectToJobDetails(jobId);
        //}
        [HttpGet]
        public IActionResult DeleteModal(Guid id, Guid? jobId, Guid? bookingId, Guid? shipmentId, Guid? shipmentLegId)
        {
            ViewBag.Id = id;
            ViewBag.JobId = jobId;
            ViewBag.BookingId = bookingId;
            ViewBag.ShipmentId = shipmentId;
            ViewBag.ShipmentLegId = shipmentLegId;

            return PartialView("_DeleteModal");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid jobId, Guid id)
        {
            await _transportDocumentService.DeleteAsync(jobId, id);

            var model = await _transportDocumentService.GetManagePageAsync(new BLContextDto
            {
                JobId = jobId,
            });

            Response.Headers["HX-Trigger"] = "blDeleted";
            return PartialView("_BLList", model);
        }


        [HttpGet]
        public async Task<IActionResult> GetBLTypeOptions(int? TransportTypeId)
        {
            var vm = new TransportDocumentUpsertVm();
            vm.BLTypes = new List<SelectListItem>();
            if (TransportTypeId.HasValue && TransportTypeId.Value != 0)
            {
                vm.BLTypes = await _lookupService.GetBLTypesByTransportTypeIdAsync(TransportTypeId.Value);
                return PartialView("_BLTypeField", vm);
            }
            return NoContent();
        }



        //[HttpPost("{id:guid}/issue")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Issue(Guid jobId, Guid id)
        {
            await _transportDocumentService.IssueAsync(jobId, id);

            TempData["SuccessMessage"] = "Transport document issued successfully.";

            return RedirectToAction(nameof(Edit), new { jobId, id });
        }

        //[HttpPost("{id:guid}/void")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Void(Guid jobId, Guid id)
        {
            await _transportDocumentService.VoidAsync(jobId, id);

            TempData["SuccessMessage"] = "Transport document voided successfully.";

            return RedirectToAction(nameof(Edit), new { jobId, id });
        }

        private async Task<TransportDocumentUpsertVm> RebuildInvalidCreateModelAsync(
            Guid jobId,
            TransportDocumentUpsertVm postedModel)
        {
            var model = await _transportDocumentService.BuildCreateModelAsync(jobId);

            CopyPostedValues(model, postedModel);

            return model;
        }

        private async Task<TransportDocumentUpsertVm> RebuildInvalidEditModelAsync(
            Guid jobId,
            Guid id,
            TransportDocumentUpsertVm postedModel)
        {
            var model = await _transportDocumentService.BuildEditModelAsync(jobId, id);

            CopyPostedValues(model, postedModel);

            return model;
        }

        private static void CopyPostedValues(
            TransportDocumentUpsertVm target,
            TransportDocumentUpsertVm source)
        {
            target.Id = source.Id;
            target.JobId = source.JobId;
            target.JobNo = source.JobNo;

            target.BookingId = source.BookingId;
            target.ShipmentId = source.ShipmentId;
            target.ShipmentLegId = source.ShipmentLegId;

            target.CompanyId = source.CompanyId;

            target.TransportTypeId = source.TransportTypeId;
            target.BLTypeId = source.BLTypeId;
            target.BLLevelId = source.BLLevelId;

            target.BLNo = source.BLNo;
            target.BLIssueDate = source.BLIssueDate;
            target.PlaceOfIssue = source.PlaceOfIssue;

            target.CarrierId = source.CarrierId;
            target.IssuerPartyId = source.IssuerPartyId;
            target.IssuedByUs = source.IssuedByUs;

            target.BLReleaseTypeId = source.BLReleaseTypeId;
            target.ReleaseDate = source.ReleaseDate;

            target.OriginalCount = source.OriginalCount;
            target.CopyCount = source.CopyCount;

            target.BLStatusId = source.BLStatusId;
            target.StatusName = source.StatusName;

            target.ExportReference = source.ExportReference;
            target.ForwardingAgentReference = source.ForwardingAgentReference;
            target.SerialNo = source.SerialNo;
            target.IsSwitched = source.IsSwitched;

            target.ShipperPartyId = source.ShipperPartyId;
            target.ShipperTextInBL = source.ShipperTextInBL;

            target.ConsigneePartyId = source.ConsigneePartyId;
            target.ConsigneeTextInBL = source.ConsigneeTextInBL;
            target.IsConsigneeToOrder = source.IsConsigneeToOrder;
            target.IsConsigneeToOrderOf = source.IsConsigneeToOrderOf;
            target.IsConsigneeToBearer = source.IsConsigneeToBearer;

            target.NotifyPartyId = source.NotifyPartyId;
            target.NotifyPartyTextInBL = source.NotifyPartyTextInBL;
            target.IsNotifySameAsConsignee = source.IsNotifySameAsConsignee;

            target.AgentPartyId = source.AgentPartyId;
            target.AgentTextInBL = source.AgentTextInBL;

            target.PORId = source.PORId;
            target.PORText = source.PORText;

            target.POLId = source.POLId;
            target.POLText = source.POLText;

            target.PODId = source.PODId;
            target.PODText = source.PODText;

            target.PODelId = source.PODelId;
            target.PODelText = source.PODelText;

            target.FinalDestinationText = source.FinalDestinationText;

            target.TranshipmentPortId = source.TranshipmentPortId;
            target.TranshipmentPortText = source.TranshipmentPortText;

            target.PointandCountryOfOriginId = source.PointandCountryOfOriginId;
            target.PointandCountryOfOriginText = source.PointandCountryOfOriginText;

            target.OnwardInlandRoutingId = source.OnwardInlandRoutingId;
            target.OnwardInlandRoutingText = source.OnwardInlandRoutingText;

            target.LoadTypeId = source.LoadTypeId;
            target.MovementTypeId = source.MovementTypeId;

            target.MarksAndNumbers = source.MarksAndNumbers;
            target.GoodsDescription = source.GoodsDescription;

            target.GrossWeightKg = source.GrossWeightKg;
            target.NetWeightKg = source.NetWeightKg;
            target.VolumeCBM = source.VolumeCBM;
            target.ChargeableWeight = source.ChargeableWeight;

            target.TotalPackages = source.TotalPackages;
            target.PackageTypeId = source.PackageTypeId;
            target.PackageTypeText = source.PackageTypeText;

            target.TotalContainers = source.TotalContainers;

            target.PaymentTypeId = source.PaymentTypeId;
            target.FreightTerms = source.FreightTerms;

            target.FreightPaymentAtId = source.FreightPaymentAtId;
            target.FreightPaymentAtText = source.FreightPaymentAtText;

            target.ShippedOnBoardDate = source.ShippedOnBoardDate;
            target.ReceivedForShipmentDate = source.ReceivedForShipmentDate;

            target.ClauseRemarks = source.ClauseRemarks;
            target.OnBoardNotation = source.OnBoardNotation;
            target.RemarksInBL = source.RemarksInBL;
            target.RemarksInManifest = source.RemarksInManifest;
            target.InternalNotes = source.InternalNotes;

            target.MRN = source.MRN;
            target.PursuitCode = source.PursuitCode;

            target.LCNo = source.LCNo;
            target.LCBankName = source.LCBankName;
            target.OrderRegistrationNo = source.OrderRegistrationNo;

            //target.VesselName = source.VesselName;
            //target.VoyageNo = source.VoyageNo;

            //target.AirlineId = source.AirlineId;
            //target.FlightNo = source.FlightNo;
            //target.FlightDate = source.FlightDate;

            //target.TruckNo = source.TruckNo;
            //target.TrailerNo = source.TrailerNo;
            //target.DriverName = source.DriverName;

            //target.TrainNo = source.TrainNo;
            //target.WagonNo = source.WagonNo;
        }

        private IActionResult RedirectToJobDetails(Guid jobId)
        {
            return RedirectToAction(
                actionName: "Details",
                controllerName: "Jobs",
                routeValues: new { id = jobId });
        }



        private void ApplyVmToBL(TransportDocumentUpsertVm model, BL entity)
        {
            entity.CompanyId = model.CompanyId ?? entity.CompanyId;

            entity.JobId = model.JobId;
            entity.BookingId = model.BookingId;
            entity.ShipmentId = model.ShipmentId;
            entity.ShipmentLegId = model.ShipmentLegId;

            entity.BLTypeId = model.BLTypeId!.Value;
            entity.BLLevelId = model.BLLevelId!.Value;
            entity.TransportTypeId = model.TransportTypeId!.Value;
            entity.BLStatusId = model.BLStatusId!.Value;

            entity.BLNo = model.BLNo ?? string.Empty;
            entity.BLIssueDate = model.BLIssueDate;
            entity.PlaceOfIssue = model.PlaceOfIssue;

            entity.CarrierId = model.CarrierId;
            entity.IssuerPartyId = model.IssuerPartyId;
            entity.IssuedByUs = model.IssuedByUs;

            entity.BLReleaseTypeId = model.BLReleaseTypeId;
            entity.ReleaseDate = model.ReleaseDate;

            entity.OriginalCount = model.OriginalCount;
            entity.CopyCount = model.CopyCount;

            entity.LoadTypeId = model.LoadTypeId;
            entity.MovementTypeId = model.MovementTypeId;

            entity.ExportReference = model.ExportReference ?? string.Empty;
            entity.ForwardingAgentReference = model.ForwardingAgentReference ?? string.Empty;
            entity.SerialNo = model.SerialNo ?? string.Empty;
            entity.IsSwitched = model.IsSwitched;

            entity.PORId = model.PORId;
            entity.PORText = model.PORText ?? string.Empty;

            entity.POLId = model.POLId;
            entity.POLText = model.POLText ?? string.Empty;

            entity.PODId = model.PODId;
            entity.PODText = model.PODText ?? string.Empty;

            entity.PODelId = model.PODelId;
            entity.PODelText = model.PODelText ?? string.Empty;

            entity.FinalDestinationText = model.FinalDestinationText ?? string.Empty;

            entity.TranshipmentPortId = model.TranshipmentPortId;

            entity.PointandCountryOfOriginId = model.PointandCountryOfOriginId;
            entity.PointandCountryOfOriginText = model.PointandCountryOfOriginText ?? string.Empty;

            entity.OnwardInlandRoutingId = model.OnwardInlandRoutingId;
            entity.OnwardInlandRoutingText = model.OnwardInlandRoutingText ?? string.Empty;

            entity.MarksAndNumbers = model.MarksAndNumbers ?? string.Empty;
            entity.GoodsDescription = model.GoodsDescription ?? string.Empty;

            entity.GrossWeightKg = model.GrossWeightKg;
            entity.NetWeightKg = model.NetWeightKg;
            entity.VolumeCBM = model.VolumeCBM;
            entity.ChargeableWeight = model.ChargeableWeight;

            entity.TotalPackages = model.TotalPackages ?? 0;
            entity.TotalContainers = model.TotalContainers ?? 0;

            entity.PackageTypeId = model.PackageTypeId;
            entity.PackageTypeText = model.PackageTypeText;

            entity.PaymentTypeId = model.PaymentTypeId;
            entity.FreightTerms = model.FreightTerms;
            entity.FreightPaymentAtId = model.FreightPaymentAtId;
            entity.FreightPaymentAtText = model.FreightPaymentAtText;

            entity.ShippedOnBoardDate = model.ShippedOnBoardDate;
            entity.ReceivedForShipmentDate = model.ReceivedForShipmentDate;

            entity.ClauseRemarks = model.ClauseRemarks;
            entity.OnBoardNotation = model.OnBoardNotation;
            entity.RemarksInBL = model.RemarksInBL ?? string.Empty;
            entity.RemarksInManifest = model.RemarksInManifest ?? string.Empty;
            entity.InternalNotes = model.InternalNotes;

            entity.MRN = model.MRN ?? string.Empty;
            entity.PursuitCode = model.PursuitCode;

            entity.LCNo = model.LCNo;
            entity.LCBankName = model.LCBankName;
            entity.OrderRegistrationNo = model.OrderRegistrationNo;
        }
        [HttpGet]
        public async Task<IActionResult> GetPartyTextInBL(Guid? PartyId)
        {
            if (!PartyId.HasValue || PartyId.Value == Guid.Empty)
            {
                return Content(string.Empty, "text/plain");
            }
            var text = await _transportDocumentService.GetPartyTextInBLAsync(PartyId.Value);
            return Content(text ?? string.Empty, "text/plain");
        }







        [HttpGet]
        public async Task<IActionResult> Split(Guid id, Guid jobId)
        {
            var vm = await _transportDocumentService.GetAdvancedSplitWorkspaceAsync(id, jobId);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAdvancedSplit([FromBody] SubmitAdvancedBLSplitVm model)
        {
            if (model == null)
            {
                return BadRequest("Invalid request.");
            }

            if (model.SourceBlId == Guid.Empty)
            {
                return BadRequest("Source BL is required.");
            }

            if (model.JobId == Guid.Empty)
            {
                return BadRequest("Job is required.");
            }

            if (model.Targets == null || !model.Targets.Any())
            {
                return BadRequest("At least one target BL is required.");
            }

            foreach (var target in model.Targets)
            {
                if (target.Assignments == null || !target.Assignments.Any())
                {
                    return BadRequest($"Target {target.TargetKey} has no assignments.");
                }

                foreach (var assignment in target.Assignments)
                {
                    if (string.IsNullOrWhiteSpace(assignment.SourceLineKey))
                    {
                        return BadRequest("Source line key is required.");
                    }

                    if (assignment.Packages <= 0 && assignment.Weight <= 0 && assignment.Volume <= 0)
                    {
                        return BadRequest("Assignment has no split values.");
                    }
                }
            }

            // TODO: اینجا منطق اصلی Split را صدا بزنید
            // await _blService.SubmitAdvancedSplitAsync(model);

            return Json(new
            {
                success = true,
                message = "Advanced BL split submitted successfully.",
                redirectUrl = Url.Action("Details", "BL", new { id = model.SourceBlId })
            });
        }



        [HttpGet]
        public async Task<IActionResult> SearchBLs(
            string? term,
            string targetHiddenInputId = "BLId",
            string targetTextInputId = "BLSearchText",
            CancellationToken cancellationToken = default)
        {
            var items = await _transportDocumentService.SearchBLsAsync(term);
            ViewBag.TargetHiddenInputId = targetHiddenInputId;
            ViewBag.TargetTextInputId = targetTextInputId;
            return PartialView("_BLSearchResults", items);
        }


        [HttpGet]
        public async Task<IActionResult> SearchVoyages(
            string? search,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(search) || search.Trim().Length < 2)
            {
                return PartialView(
                    "_VoyageLookupResults",
                    Array.Empty<VoyageLookupItemVm>());
            }

            var voyages = await _voyageService.SearchForSelectAsync(
                search.Trim(),
                take: 25,
                cancellationToken);

            var model = voyages.Select(x => new VoyageLookupItemVm
            {
                Id = x.Id,
                VesselName = x.VesselName,
                VoyageNo = x.VoyageNo
            }).ToList();

            return PartialView("_VoyageLookupResults", model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchTrips(
            string? search,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(search) || search.Trim().Length < 2)
            {
                return PartialView(
                    "_TripLookupResults",
                    Array.Empty<TripLookupItemVm>());
            }

            var trips = await _tripService.SearchForSelectAsync(
                search.Trim(),
                take: 25,
                cancellationToken);

            var model = trips.Select(x => new TripLookupItemVm
            {
                Id = x.Id,
                TruckNo = x.TruckNo,
                TrailerNo = x.TrailerNo,
                DriverName = x.DriverName,
                CMRNo = x.CMRNo,
                TIRNo = x.TIRNo
            }).ToList();
            return PartialView("_TripLookupResults", model);
        }



        [HttpGet]
        public async Task<IActionResult> SearchFlights(
            string? search,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(search) || search.Trim().Length < 2)
            {
                return PartialView(
                    "_FlightLookupResults",
                    Array.Empty<FlightLookupItemVm>());
            }

            var flights = await _flightService.SearchForSelectAsync(
                search.Trim(),
                take: 25,
                cancellationToken);

            var model = flights.Select(x => new FlightLookupItemVm
            {
                Id = x.Id,
                AirlineId = x.AirlineId,
                AirlineName = x.AirlineName,
                FlightNo = x.FlightNo,
                OriginAirportName = x.OriginAirportName,
                DestinationAirportName = x.DestinationAirportName
            }).ToList();

            return PartialView("_FlightLookupResults", model);
        }


        [HttpGet]
        public async Task<IActionResult> SearchFlightDates(
            Guid flightId,
            CancellationToken cancellationToken)
        {
            if (flightId == Guid.Empty)
            {
                return PartialView(
                    "_FlightDateLookupResults",
                    Array.Empty<FlightDateLookupItemVm>());
            }

            var flightDates = await _flightService.GetForFlightSelectAsync(
                flightId,
                take: 50,
                cancellationToken);

            var model = flightDates.Select(x => new FlightDateLookupItemVm
            {
                Id = x.Id,
                ETD = x.ETD,
                ETA = x.ETA,
                ActualDeparture = x.ActualDeparture,
                ActualArrival = x.ActualArrival,
                DelayHours = x.DelayHours
            }).ToList();
            return PartialView("_FlightDateLookupResults", model);
        }
        public async Task<IActionResult> BLSeaReport(Guid id, CancellationToken cancellationToken = default)
        {
            var pdf = await _manifestReportService.GenerateSeaBillOfLadingPdfAsync(id, false, cancellationToken);
            Response.Headers.ContentDisposition =
                $"inline; filename=\"{"SeaBL.pdf"}\"";
            return File(pdf, "application/pdf");
        }






        [HttpGet]
        public async Task<IActionResult> GetContainerAllocationModal(Guid blId, Guid roadTripId, CancellationToken cancellationToken)
        {
            try
            {
                var vm = await _transportDocumentService.GetContainerAllocationModalAsync(blId, roadTripId, cancellationToken);
                return PartialView("_ContainerAllocationModalPartial", vm);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading allocation modal: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveContainerAllocation(SaveContainerAllocationDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid allocation payload.");
            }

            var result = await _transportDocumentService.SaveContainerAllocationAsync(dto, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(result.Message);
            }

            // ارسال سیگنال بسته شدن مودال و نمایش توست از طریق هدر HTMX
            Response.Headers.Append("HX-Trigger",
                "{\"allocationUpdated\": true, \"showToast\": \"Containers successfully allocated to the trip.\"}");
            return Content("<div class='modal-body text-success text-center py-4'><i class='bi bi-check-circle fs-1'></i><p class='mt-2 mb-0 fw-bold'>Saved successfully!</p></div>");
        }











    }
}
