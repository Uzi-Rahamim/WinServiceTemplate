using AsyncPipeTransport.CommonTypes;
using CommunicationMessages;


namespace CommTypes.Massages
{ 
    public class RequestSecurityMessage : MessageHeader
    {
        public string token { get; set; }

        public RequestSecurityMessage(string token) : base((Opcode)0) => this.token = token;
    }
    public class ResponseSecurityMessage : MessageHeader
    {
        public bool isValid { get; set; }
        public ResponseSecurityMessage(bool isValid) : base((Opcode)0) => this.isValid = isValid;
    }
}
