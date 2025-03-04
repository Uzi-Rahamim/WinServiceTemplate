namespace Intel.IntelConnect.IPC.CommonTypes.InternalMassages
{
    public class ErrorMessage : MessageHeader
    {
        public string Message { get; set; }
        public int Code { get; set; }
        public ErrorMessage(string message, int code) 
        {
            Message = message;
            Code = code;
        }
    }

    public enum ErrorCode
    {
        RequestTimeout = 408,
        RequestEntityTooLarge = 413,
        InternalServerError = 500, 
        NotImplemented = 501,
        ServiceUnavailable = 503
    }
}
