namespace ShipRate.Application.Utilities
{
    public class ServiceResult
    {
        public bool Succeeded { get; set; }

        public string Message { get; set; } = string.Empty;

        public List<string> Errors { get; set; } = new();

        public string FormattedErrors(string separator = "-")
        {
           return string.Join(separator, Errors);
        }
        public static ServiceResult Success(string message = "")
        {
            return new ServiceResult
            {
                Succeeded = true,
                Message = message
            };
        }

        public static ServiceResult Failed(string error)
        {
            return new ServiceResult
            {
                Succeeded = false,
                Errors = new List<string> { error }
            };
        }

        public static ServiceResult Failed(IEnumerable<string> errors)
        {
            return new ServiceResult
            {
                Succeeded = false,
                Errors = errors.ToList()
            };
        }


    }

    public class ServiceResult<T>:ServiceResult
    {
        public bool Succeeded { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public List<string> Errors { get; set; } = new();

        public static ServiceResult<T> Success(T data, string message = "")
        {
            return new ServiceResult<T>
            {
                Succeeded = true,
                Data = data,
                Message = message
            };
        }

        public static ServiceResult<T> Failed(string error)
        {
            return new ServiceResult<T>
            {
                Succeeded = false,
                Errors = new List<string> { error }
            };
        }

        public static ServiceResult<T> Failed(IEnumerable<string> errors)
        {
            return new ServiceResult<T>
            {
                Succeeded = false,
                Errors = errors.ToList()
            };
        }
    }


}
