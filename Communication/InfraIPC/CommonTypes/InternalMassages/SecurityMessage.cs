using Intel.IntelConnect.IPC.CommonTypes;


namespace Intel.IntelConnect.IPCCommon.Massages
{

    public class RequestSecurityMessage : IMessageHeader
    {
        public string token { get; set; }

        public RequestSecurityMessage(string token) => this.token = token;
    }

    public class ResponseSecurityMessage : IMessageHeader
    {
        public bool isValid { get; set; }
        public string hostVersion { get; set; }
        public ResponseSecurityMessage(bool isValid,string hostVersion) => (this.isValid, this.hostVersion) = (isValid, hostVersion);
    }
}
