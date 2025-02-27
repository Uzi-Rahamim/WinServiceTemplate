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
        public string hostVersion { get; set; }
        public ResponseSecurityMessage(bool isValid,string hostVersion) : base(String.Empty) => (this.isValid, this.hostVersion) = (isValid, hostVersion);
    }
}
