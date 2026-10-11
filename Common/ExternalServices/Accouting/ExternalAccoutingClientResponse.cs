namespace ShipRate.Common.ExternalServices.Accouting
{
    public class ExternalAccoutingClientResponse<T>
    {
        public ExternalAccoutingClientResponse(int statusCode, string message, string errorCode, T data)
        {
            StatusCode = statusCode;
            Message = message;
            ErrorCode = errorCode;
            Data = data;
        }

        public ExternalAccoutingClientResponse()
        {

        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public T Data { get; set; }
    }


    public class ExternalAccoutingClientResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
    }
}

