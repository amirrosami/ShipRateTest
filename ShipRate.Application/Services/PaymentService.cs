using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Services.Finance;
using ShipRate.Application.ViewModels;
using ShipRate.Common.ExternalServices.PaymentGateways;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Infrastructure.Data;
using ShipRate.Infrastructure.Repositories.Queries;

namespace ShipRate.Application.Services
{
    public class PaymentService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService currentUserService;
        private readonly PaymentDocumentService paymentDocumentService;
        private readonly SettlementService settlementService;
        private readonly FinanceQueries financeQueries;
        public PaymentService(ApplicationDbContext db, ICurrentUserService currentUserService, PaymentDocumentService paymentDocumentService, SettlementService settlementService, FinanceQueries financeQueries)
        {
            this.settlementService = settlementService;
            this.paymentDocumentService = paymentDocumentService;
            _db = db;
            this.currentUserService = currentUserService;
            this.financeQueries = financeQueries;
        }


        public async Task<Utilities.ServiceResult<OnlinePaymentListPageVm>> GetPagedOnlinePayments(OnlinePaymentFilterVm vm)
        {

            var currentPersonId = await currentUserService.GetPersonIdAsync();
            var query = _db.OnlinePayments
                .Where(x => !x.IsDeleted && x.CustomerId == currentPersonId)
                .Include(x => x.Terminal)
                .Include(x => x.Status)
                .Include(x => x.Items).ThenInclude(x => x.Invoice)
                .AsQueryable<OnlinePayment>();

            if (!string.IsNullOrEmpty(vm.RequestNo))
            {
                query = query.Where(x => x.Items.Any(i => i.Invoice.InvoiceNo.Contains(vm.InvoiceNo)));
            }
            vm.PageNumber = vm.PageNumber < 1 ? 1 : vm.PageNumber;
            vm.PageSize = vm.PageSize <= 0 ? 10 : vm.PageSize;
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.RequestNo)
                .Select(x => new OnlinePaymentListVM
                {
                    Id = x.Id,
                    RequestNo = x.RequestNo,
                    BankReferenceNo = x.BankReferenceNo,
                    TraceNo = x.TraceNo,
                    Status = x.Status.OnlinePaymentStatusName,
                    TotalPayment = x.Items.Sum(i => i.PaymentAmount)
                })
                .Skip((vm.PageNumber - 1) * vm.PageSize)
                .Take(vm.PageSize)
                .ToListAsync();

            return Utilities.ServiceResult<OnlinePaymentListPageVm>.Success(new OnlinePaymentListPageVm
            {
                Result = new PagedResultVm<OnlinePaymentListVM> { Items = items, TotalCount = totalCount, PageNumber = vm.PageNumber, PageSize = vm.PageSize },
                Filter = new OnlinePaymentFilterVm { }
            });
        }


        public async Task<Utilities.ServiceResult<GatewayCallbackResultVM>> HandleCallback(GatewayType gatewayType, object responseDto)
        {
            var gateway = new PaymentGatewayResolver().ResolveByType(gatewayType);
            if (gateway == null)
            {
                return Utilities.ServiceResult<GatewayCallbackResultVM>.Failed("invalid gateway");
            }

            var callbackResult = gateway.ParseCallbackResult(responseDto);

            if (callbackResult.Succeded)
            {
                var result = await HandleCallbackResultSucceded(gateway, callbackResult);
                return Utilities.ServiceResult<GatewayCallbackResultVM>.Success(new GatewayCallbackResultVM { Message = "Payment was Success", Amount = callbackResult.Amount, RefNumber = callbackResult.RefNum, IsSuccess = true });
            }

            else
            {
                await HandleCallbackResultFailed(gateway, callbackResult);
            }

            return Utilities.ServiceResult<GatewayCallbackResultVM>.Failed("Payment was Rejected");
        }
        private async Task<bool> HandleCallbackResultFailed(PaymentGateway gateway, PaymentCallbackResult callbackResult)
        {
            var onlinepayment = await GetOnlinePayment(Guid.Parse(callbackResult.ResNum));
            await SaveOnlinePaymentRejected(onlinepayment);
            return true;
        }

        private async Task<bool> HandleCallbackResultSucceded(PaymentGateway gateway, PaymentCallbackResult callbackResult)
        {
            var onlinepayment = await GetOnlinePayment(Guid.Parse(callbackResult.ResNum));
            if (onlinepayment == null)
            {
                return false;
                // payment is invalid
                // send Failed to Gateway
            }
            var saved = await SaveOnlinePaymentRequested(onlinepayment);
            if (saved)
            {
                var verifyReslt = await gateway.VerifyTransaction(callbackResult);
                if (verifyReslt.Succeded)
                {
                    var verifySaved = await SaveOnlinePaymentVerified(onlinepayment);
                    if (verifySaved)
                    {
                        var savedDocs = await HandlePaymentDocumentAndSettlement(onlinepayment);
                        if (savedDocs)
                        {
                            await SaveOnlinePaymentConfirmed(onlinepayment);
                        }
                    }
                }
            }
            return true;
        }

        private async Task<bool> HandlePaymentDocumentAndSettlement(OnlinePayment onlinePayment)
        {
            var currentUser = currentUserService.UserId;
            // start transaction 
            var customerInfo = await _db.Persons.Where(x => x.Id == onlinePayment.CustomerId).FirstOrDefaultAsync();
            await _db.Database.BeginTransactionAsync();

            try
            {
                var createPamentDocumentVM = new PaymentDocumentCreateEditVm
                {
                    Id = Guid.NewGuid(),
                    CompanyId = onlinePayment.Terminal.CompanyId,
                    DocumentNumber = onlinePayment.Id.ToString(),
                    PaymentMethodId = 8,//online payment
                    FinanceDirectionId = 1,// Income
                    PersonId = onlinePayment.CustomerId,
                    CurrencyId = 2, // RIAL
                    Amount = onlinePayment.Items.Sum(x => x.PaymentAmount),
                    DocumentDateGregorian = DateTime.Now,
                    TrackingNumber = onlinePayment.TraceNo
                };
                var id = await paymentDocumentService.CreateAsync(createPamentDocumentVM);
                var settlementCreateDto = new SettlementHeaderCreateDto
                {
                    CompanyId = onlinePayment.Terminal.CompanyId,
                    PartyId = onlinePayment.CustomerId,
                    PartyName = customerInfo.PersonName,
                    Remarks = "",
                    StatusId = 2,
                    SettlementDate = DateTime.Now,
                    CurrencyId = createPamentDocumentVM.CurrencyId ?? 2
                };

                var settlmentId = await settlementService.CreateHeaderAsync(settlementCreateDto, currentUser);
                foreach (var item in onlinePayment.Items)
                {
                    var settlmentItemDto = new SettlementItemCreateDto
                    {
                        SettlementId = settlmentId,
                        InvoiceId = item.InvoiceId,
                        Amount = item.PaymentAmount
                    };

                    var itemID = await settlementService.AddItemAsync(settlmentItemDto, currentUser);
                    var paymentAllocationDto = new PaymentAllocationCreateDto
                    {
                        SettlementId = settlmentId,
                        SettlementItemId = itemID,
                        PaymentDocumentId = id,
                        AllocatedAmount = item.PaymentAmount,
                        AllocatedAmountInBaseCurrency = item.PaymentAmount,
                        ExchangeRateValue = 1
                    };

                    await settlementService.AddPaymentAllocationAsync(paymentAllocationDto, currentUser);

                }

                await _db.Database.CommitTransactionAsync();
                return true;
            }
            catch (Exception)
            {

                await _db.Database.RollbackTransactionAsync();
                return false;
            }

            // add payment document
            // add sttlement
            // save
            // save onlinepayment as Finally Confirmed
            // commit transaction
        }



        private async Task<bool> SaveOnlinePaymentRejected(OnlinePayment onlinePayment)
        {
            onlinePayment.StatusId = 80;
            _db.OnlinePayments.Update(onlinePayment);
            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> SaveOnlinePaymentRequested(OnlinePayment onlinePayment)
        {
            onlinePayment.StatusId = 20;
            _db.OnlinePayments.Update(onlinePayment);
            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private async Task<bool> SaveOnlinePaymentVerified(OnlinePayment onlinePayment)
        {
            onlinePayment.StatusId = 30;
            _db.OnlinePayments.Update(onlinePayment);
            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> SaveOnlinePaymentConfirmed(OnlinePayment onlinePayment)
        {
            onlinePayment.StatusId = 40; //finally confirmed
            _db.OnlinePayments.Update(onlinePayment);
            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<Utilities.ServiceResult<CreateOnlinePaymentResultDto>> CreateOnlinePayment(CreateOnlinePaymentVm vm)
        {
            var personId = await currentUserService.GetPersonIdAsync();
            if (personId == null || personId == Guid.Empty)
            {
                return Utilities.ServiceResult<CreateOnlinePaymentResultDto>.Failed("Invalid Customer!!");
            }
            var invoices = await _db.Invoices.Where(i => vm.Items.Select(x => x.InvoiceId).Contains(i.Id)).ToListAsync();

            if (invoices.Count != vm.Items.Count)
            {
                return Utilities.ServiceResult<CreateOnlinePaymentResultDto>.Failed("Invalid Invoices selected for payment");
            }
            if (invoices.Any(x => x.PartyId != personId))
            {
                return Utilities.ServiceResult<CreateOnlinePaymentResultDto>.Failed("You should pay invoices that owns to this customer");
            }

            // check that all payments for that invoices completed and sttlements created .
            // check remain payment for each payment

            var remains = await financeQueries.GetRemainInvoicePayment().Where(x => invoices.Select(i => i.Id).Contains(x.InvoiceId)).ToListAsync();
            var remainPaymentsInvoices =
                from rp in remains
                join payment in vm.Items on rp.InvoiceId equals payment.InvoiceId
                where rp.RemainPayment < payment.PaymentAmount
                select rp.InvoiceId;

            if (remainPaymentsInvoices != null && remainPaymentsInvoices.Count() > 0)
            {
                return Utilities.ServiceResult<CreateOnlinePaymentResultDto>.Failed("Remain Payment is greater that Payment Amount");
            }
            var ipgTerminal = await GetTerminalInfoAsync(vm.IPGTerminalId);

            if (ipgTerminal == null || string.IsNullOrEmpty(ipgTerminal.IPGTerminalId))
            {
                return Utilities.ServiceResult<CreateOnlinePaymentResultDto>.Failed("Invalid Terminal for online payment");
            }

            if (!ipgTerminal.IPGAccounts.Any())
            {
                return Utilities.ServiceResult<CreateOnlinePaymentResultDto>.Failed("Error in Terminal Configurations");
            }

            var paymentGateway = new PaymentGatewayResolver().ResolveByInfo(new PaymentGatewayInfo
            {
                GatewayId = ipgTerminal.IPGTerminalId,
                GatewayName = ipgTerminal.IPGTerminalName
            });

            var callbackUrl = await GetCallBackUrlAsync();

            var onlinePaymentId = Guid.NewGuid(); // PaymentId used as ResNum
            var paymentInfo = new PaymentInfo
            {
                Amount = vm.Items.Sum(x => x.PaymentAmount),
                TerminalId = ipgTerminal.IPGTerminalId,
                CellNumber = "",
                CallBackUrl = callbackUrl,
                ResNum = onlinePaymentId.ToString(),
            };


            var paymentGWInitResult = await paymentGateway.InitializePayment(paymentInfo);
            // get token from Payment Gateway:
            if (!paymentGWInitResult.Succeded)
            {
                return Utilities.ServiceResult<CreateOnlinePaymentResultDto>.Failed("Error from Payment Gateway!!");
            }


            // if gettoken was successfull => create draft online payment with items:
            var onlinePayment = new OnlinePayment
            {
                Id = onlinePaymentId,
                RequestNo = _db.OnlinePayments.Count() + 1,
                StatusId = 10, // draft
                CustomerId = personId.Value,
                IPGTerminalId = ipgTerminal.Id,
                BankReferenceNo = paymentGWInitResult.Token,
                BankId = ipgTerminal.IPGAccounts.First().BankAccount.Bank.Id,
                CreatedAt = DateTime.UtcNow,
                CreateUserId = currentUserService.UserId,
            };
            await _db.Database.BeginTransactionAsync();
            _db.OnlinePayments.Add(onlinePayment);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {

                await _db.Database.RollbackTransactionAsync();
                return Utilities.ServiceResult<CreateOnlinePaymentResultDto>.Failed("Error in create online Payment!");
            }
            var items = vm.Items.Select(x => new OnlinePaymentItem
            {
                Id = Guid.CreateVersion7(),
                OnlinePaymentId = onlinePayment.Id,
                InvoiceId = x.InvoiceId,
                PaymentAmount = x.PaymentAmount,
                RemainAmount = x.RemainAmount,
            });
            _db.OnlinePaymentItems.AddRange(items);
            try
            {
                await _db.SaveChangesAsync();
                await _db.Database.CommitTransactionAsync();
            }
            catch (Exception)
            {

                await _db.Database.RollbackTransactionAsync();
                return Utilities.ServiceResult<CreateOnlinePaymentResultDto>.Failed("Error in create online Payment!");
            }
            // send result and token :


            var result = new CreateOnlinePaymentResultDto
            {
                Token = paymentGWInitResult.Token,
                RedirectUrl = paymentGWInitResult.RedirectUrl,
                ResNum = onlinePaymentId.ToString(),
            };
            return Utilities.ServiceResult<CreateOnlinePaymentResultDto>.Success(result);
        }

        public async Task<OnlinePayment> GetOnlinePayment(Guid id)
        {
            var onlinepayment = await _db.OnlinePayments.Where(x => x.Id == id)
                .Include(d => d.Items).Include(x => x.Terminal).FirstOrDefaultAsync();
            return onlinepayment;
        }

        public async Task<Utilities.ServiceResult<List<IPGTerminal>>> GetAvailableIPGTerminals()
        {
            var list = await _db.IPGTerminals.ToListAsync();
            if (list == null || list.Count == 0)
            {
                return Utilities.ServiceResult<List<IPGTerminal>>.Failed("there is no available gateway for online payment");
            }
            return Utilities.ServiceResult<List<IPGTerminal>>.Success(list);
        }
        private async Task<string> GetCallBackUrlAsync()
        {
            return "https://localhost:5001/paymentcallback";
        }



        private async Task<IPGTerminal> GetTerminalInfoAsync(Guid ipgTerminalId)
        {
            var terminal = await _db.IPGTerminals.Where(x => x.Id == ipgTerminalId)
               .Include(x => x.IPGAccounts).ThenInclude(c => c.BankAccount).ThenInclude(d => d.Bank).FirstOrDefaultAsync();
            return terminal;
        }
        private async Task<Bank> GetBankForPay()
        {
            var bank = await _db.Banks.Where(x => x.BankName == "Saman").FirstOrDefaultAsync();
            return bank;
        }


        //private async Task<long> GetRemainPayment(List<Guid> invoiceIds)
        //{
        //   var remianPayments = _db.OnlinePaymentItems
        //        .Include(x=>x.OnlinePayment)
        //        .Where(x=>x.OnlinePayment.StatusId == 40)
        //        .Join(_db.Invoices ,o=>o.InvoiceId , i=>i.Id , (inner,outer) => (inner,outer))
        //        .GroupBy(x=>x.inner.InvoiceId)
        //        .Aggregate

        //        ;
        //}
    }
}