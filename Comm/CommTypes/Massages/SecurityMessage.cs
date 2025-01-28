using AsyncPipeTransport.CommonTypes;


namespace CommTypes.Massages
{

    public class RequestSecurityMessage : MessageHeader
    {
        public string token { get; set; }

        public RequestSecurityMessage(string token) : base(String.Empty) => this.token = token;
    }
    public class ResponseSecurityMessage : MessageHeader
    {
        public bool isValid { get; set; }
        public ResponseSecurityMessage(bool isValid) : base(String.Empty) => this.isValid = isValid;
    }
}
