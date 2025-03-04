using Intel.IntelConnect.IPC.CommonTypes;


namespace Intel.IntelConnect.IPCCommon.Massages
{

    public class RequestSecurityMessage : MessageHeader
    {
        public string token { get; set; }

        public RequestSecurityMessage(string token) => this.token = token;
    }

    public class ResponseSecurityMessage : MessageHeader
    {
        public bool isValid { get; set; }
        public string hostVersion { get; set; }
        public ResponseSecurityMessage(bool isValid,string hostVersion) => (this.isValid, this.hostVersion) = (isValid, hostVersion);
    }
}
