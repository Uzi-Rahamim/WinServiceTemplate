namespace AsyncPipeTransport.Exceptions
{
    public class ErrorResponseException : Exception
    {
        public int ErrorCode { get; set; }

        public ErrorResponseException(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
