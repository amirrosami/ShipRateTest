using System;
using System.Collections.Generic;
using System.Text;

namespace ShipRate.Common.ExternalServices.PaymentGateways
{
    public class PaymentGatewayInfo
    {
        public string GatewayName { get; set; }
        public string GatewayId { get; set; }
    }
    public enum GatewayType
    {
        SAMAN =0,
        MELLAT= 1
    }
   

    public class PaymentGatewayResolver 
    {
        public PaymentGateway ResolveByInfo(PaymentGatewayInfo gatewayInfo)
        {

            switch (gatewayInfo.GatewayName)
            {
                case "SAMAN":
                    return new SamanPaymentGateway();
                case "Mellat":
                    return new MellatPaymentGateway();
                default:
                    throw new ArgumentNullException();
            }
        }
        public PaymentGateway ResolveByType(GatewayType gatewayType)
        {

            switch (gatewayType)
            {
                case GatewayType.SAMAN:
                    return new SamanPaymentGateway();
                case GatewayType.MELLAT:
                    return new MellatPaymentGateway();
                default:
                    throw new ArgumentNullException();
            }
        } 
    }

    public class InitializeGatewayPaymentResult
    {
        public bool Succeded { get; set; }
        public int Status { get; set; }
        public string Token { get; set; }
        public string ExtraInfo { get; set; }

        public string RedirectUrl {  get; set; }
    }
    public class PaymentInfo
    {
        public string TerminalId { get; set; }
        public decimal Amount { get; set; }
        public string CallBackUrl { get; set; }
        public string CellNumber { get; set; }
        public string ResNum { get; set; }
    }
    public abstract class PaymentGateway
    {
        public abstract Task<InitializeGatewayPaymentResult> InitializePayment(PaymentInfo paymentInfo);
        public abstract PaymentCallbackResult ParseCallbackResult(object resultDto);

        public abstract Task<VerifyTransactionResult> VerifyTransaction(PaymentCallbackResult callbackResult);
        public abstract Task<VerifyTransactionResult> VerifyTransactionFailed(PaymentCallbackResult callbackResult);
    }

    public class VerifyTransactionResult
    {
        public bool Succeded { get; set; }
    }
    public class SamanPaymentCallBackResponseDto
    {
        public string MID { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public string RRN { get; set; }
        public string RefNum { get; set; }
        public string ResNum { get; set; }
        public string TerminalId { get; set; }
        public string TraceNo { get; set; }
        public decimal Amount { get; set; }
        public string Wage { get; set; }
        public string SecurePAN { get; set; }
        public string HashedCardNumber { get; set; }
    }


    public class PaymentCallbackResult
    {
        public string SecurePAN { get; set; }
        public string HashedCardNumber { get; set; }
        public string RefNum { get; set; }
        public string TerminalId { get; set; }
        public decimal Amount { get; set; }
        public string ResNum { get; set; }
        public bool Succeded { get; set; }
        public Dictionary<string, string?> ExtraProperties { get; init; }
        = new(StringComparer.OrdinalIgnoreCase);
    }
    public class SamanPaymentGateway : PaymentGateway
    {

        public override async Task<InitializeGatewayPaymentResult> InitializePayment(PaymentInfo paymentInfo)
        {
            string token = Guid.NewGuid().ToString();
            string redirectUrl = $"https://localhost:5001/FakePaymentGW/index?token={token}&amount={paymentInfo.Amount}&ResNum={paymentInfo.ResNum}";
            return new InitializeGatewayPaymentResult {Succeded=true,Status = 1, Token = token ,RedirectUrl = redirectUrl};
        }

        public override PaymentCallbackResult ParseCallbackResult(object resultDto)
        {
            SamanPaymentCallBackResponseDto dto = (SamanPaymentCallBackResponseDto)resultDto;
            return new PaymentCallbackResult
            {
                RefNum = dto.RefNum,
                TerminalId = dto.TerminalId,
                Amount = dto.Amount,
                ResNum = dto.ResNum,
                SecurePAN = dto.SecurePAN,
                HashedCardNumber = dto.HashedCardNumber,
                Succeded = IsSuccess(dto.State)
            };           
        }

        public override async Task<VerifyTransactionResult> VerifyTransaction(PaymentCallbackResult callbackResult)
        {
            return new VerifyTransactionResult
            {
                Succeded = true
            };
        }

        public override async Task<VerifyTransactionResult> VerifyTransactionFailed(PaymentCallbackResult callbackResult)
        {
            return new VerifyTransactionResult
            {
                Succeded = true
            };
        }

        private bool IsSuccess(string state)
        {
            if (state == "OK")
            {
                return true;
            }
            return false;
        }
    }

    public class MellatPaymentGateway : PaymentGateway
    {
        public override async Task<InitializeGatewayPaymentResult> InitializePayment(PaymentInfo paymentInfo)
        {
            throw new NotImplementedException();
        }

        public override PaymentCallbackResult ParseCallbackResult(object resultDto)
        {
            throw new NotImplementedException();
        }

        public override Task<VerifyTransactionResult> VerifyTransaction(PaymentCallbackResult callbackResult)
        {
            throw new NotImplementedException();
        }

        public override Task<VerifyTransactionResult> VerifyTransactionFailed(PaymentCallbackResult callbackResult)
        {
            throw new NotImplementedException();
        }
    }
}
